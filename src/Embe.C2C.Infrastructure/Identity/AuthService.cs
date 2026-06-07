using System.Text;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Embe.C2C.Infrastructure.Identity;

public class AuthService
(
    IConfiguration configuration,
    Ef.Contexts.C2CContext context,
    IAuthenticatedUserService userService,
    SignInManager<MyIdentityUser> signInManager
) : IAuthService
{
    private readonly Ef.Contexts.C2CContext _context = context;
    private readonly SignInManager<MyIdentityUser> _signInManager = signInManager;
    private readonly IAuthenticatedUserService _userService = userService;
    private readonly string _jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
    private readonly string _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
    private readonly string _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret is not configured.");

    private static readonly TimeSpan _accessTokenLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<TypedResult<RefreshFailureReason, Credentials>> RefreshAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        try
        {

            var principal = tokenHandler.ValidateToken(refreshTokenValue, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var refreshTokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
            if (refreshTokenIdClaim == null || !Guid.TryParse(refreshTokenIdClaim.Value, out var refreshTokenId))
            {
                return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.InvalidRefreshToken, "Invalid refresh token.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var refreshTokenEntity = await _context.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(rt => rt.Id == refreshTokenId, cancellationToken);
            if (refreshTokenEntity == null)
            {
                return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.InvalidRefreshToken, "Invalid refresh token.");
            }

            var refreshTokenHasExpired = refreshTokenEntity.ExpiresAt < DateTimeOffset.UtcNow;
            if (refreshTokenHasExpired)
            {
                return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.ExpiredRefreshToken, "Refresh token has expired.");
            }

            var user = await _context.DomainUsers.AsNoTracking().SingleOrDefaultAsync(u => u.Id == refreshTokenEntity.UserId, cancellationToken);
            if (user == null)
            {
                return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.InvalidRefreshToken, "Invalid refresh token.");
            }

            var identityUser = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == user.Email.Value, cancellationToken);
            if (identityUser == null)
            {
                return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.InvalidRefreshToken, "Invalid refresh token.");
            }

            var refreshToken = new RefreshToken(refreshTokenId, refreshTokenValue, refreshTokenEntity.ExpiresAt);
            var accessToken = GenerateAccessToken(refreshToken, identityUser, user);
            var credentials = new Credentials(accessToken, refreshToken);

            return TypedResult<RefreshFailureReason, Credentials>.Success(credentials);
        }
        catch (Exception)
        {
            return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.InvalidRefreshToken, "Invalid refresh token.");
        }
    }

    public async Task<TypedResult<SignInFailureReason, Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var signInResult = await _signInManager.PasswordSignInAsync(email, password, false, true);
        if (!signInResult.Succeeded)
        {
            return signInResult switch
            {
                { IsLockedOut: true } => TypedResult<SignInFailureReason, Credentials>.Failure(SignInFailureReason.TooManyAttempts, "Too many failed attempts. Please try again later."),
                { IsNotAllowed: true } => TypedResult<SignInFailureReason, Credentials>.Failure(SignInFailureReason.UserNotConfirmed, "User account is not confirmed."),
                _ => TypedResult<SignInFailureReason, Credentials>.Failure(SignInFailureReason.InvalidCredentials, "Invalid email or password.")
            };
        }

        var identityUser = await _context.Users.SingleAsync(u => u.Email == email, cancellationToken);
        var user = await _context.DomainUsers.SingleAsync(u => u.Email.Value == email, cancellationToken);
        if (user == null)
        {
            return TypedResult<SignInFailureReason, Credentials>.Failure(SignInFailureReason.UserNotFound, "User not found.");
        }

        var credentials = GenerateCredentials(identityUser, user);
        var refreshTokenEntity = new RefreshTokenEntity(credentials.RefreshToken.Id, user.Id, credentials.RefreshToken.ExpiresAt);
        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return TypedResult<SignInFailureReason, Credentials>.Success(credentials);
    }

    public async Task<TypedResult<SignOutFailureReason, bool>> SignOutAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var userId = _userService.UserId;
        if (userId == null)
        {
            return TypedResult<SignOutFailureReason, bool>.Failure(SignOutFailureReason.Unauthorized, "User is not authenticated.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var refreshToken = ParseRefreshToken(refreshTokenValue);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshToken.Id && rt.UserId == userId, cancellationToken);
        if (refreshTokenEntity == null)
        {
            return TypedResult<SignOutFailureReason, bool>.Failure(SignOutFailureReason.Unauthorized, "Refresh token not found for the user.");
        }
        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return TypedResult<SignOutFailureReason, bool>.Success(true);
    }

    private Credentials GenerateCredentials(IdentityUser identityUser, User user)
    {
        var refreshToken = GenerateRefreshToken(identityUser, user);
        var accessToken = GenerateAccessToken(refreshToken, identityUser, user);
        return new Credentials(accessToken, refreshToken);
    }

    private AccessToken GenerateAccessToken(RefreshToken refreshToken, IdentityUser identityUser, User user)
    {
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new System.Security.Claims.Claim("sub", identityUser.Id),
            new System.Security.Claims.Claim("userId", user.Id.ToString()),
            new System.Security.Claims.Claim("refreshTokenId", refreshToken.Id.ToString()),
        };

        var jwtSecurityToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken
        (
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_accessTokenLifetime),
            signingCredentials: credentials
        );

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return new AccessToken(token, DateTimeOffset.UtcNow.Add(_accessTokenLifetime));
    }

    private RefreshToken GenerateRefreshToken(IdentityUser identityUser, User user)
    {
        var tokenId = Guid.CreateVersion7();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new System.Security.Claims.Claim("sub", identityUser.Id),
            new System.Security.Claims.Claim("userId", user.Id.ToString()),
            new System.Security.Claims.Claim("tokenId", tokenId.ToString()),
        };

        var jwtSecurityToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken
        (
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_refreshTokenLifetime),
            signingCredentials: credentials
        );

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return new RefreshToken(tokenId, token, DateTimeOffset.UtcNow.Add(_refreshTokenLifetime));
    }

    public async Task<TypedResult<InvalidateRefreshTokenFailureReason, bool>> InvalidateRefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken)
    {
        var userId = _userService.UserId;
        if (userId == null)
        {
            return TypedResult<InvalidateRefreshTokenFailureReason, bool>.Failure(InvalidateRefreshTokenFailureReason.Unauthorized, "User is not authenticated.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var refreshToken = ParseRefreshToken(refreshTokenValue);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshToken.Id && rt.UserId == userId, cancellationToken);
        if (refreshTokenEntity == null)
        {
            return TypedResult<InvalidateRefreshTokenFailureReason, bool>.Failure(InvalidateRefreshTokenFailureReason.Unauthorized, "Refresh token not found for the user.");
        }
        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return TypedResult<InvalidateRefreshTokenFailureReason, bool>.Success(true);
    }

    private RefreshToken ParseRefreshToken(string refreshTokenValue)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var principal = tokenHandler.ReadJwtToken(refreshTokenValue);

        var refreshTokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        var expiresAt = principal.ValidTo;
        if (refreshTokenIdClaim == null || !Guid.TryParse(refreshTokenIdClaim.Value, out var refreshTokenId))
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        return new RefreshToken(refreshTokenId, refreshTokenValue, expiresAt);
    }

    public async Task<bool> AccountExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
}