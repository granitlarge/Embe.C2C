using System.Text;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Commands.Users.Handlers;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Embe.C2C.Infrastructure.Identity;

public class AuthService
(
    Settings settings,
    Ef.Contexts.C2CContext context,
    IAuthenticatedUserService userService,
    SignInManager<MyIdentityUser> signInManager,
    UserManager<MyIdentityUser> userManager
) : IAuthService
{
    private readonly Ef.Contexts.C2CContext _context = context;
    private readonly SignInManager<MyIdentityUser> _signInManager = signInManager;
    private readonly UserManager<MyIdentityUser> _userManager = userManager;
    private readonly IAuthenticatedUserService _userService = userService;
    private readonly Settings _settings = settings;
    private static readonly TimeSpan _accessTokenLifetime = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<TypedResult<RefreshFailureReason, Credentials>> RefreshAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSettings.RefreshTokenSecret));
        try
        {

            var principal = tokenHandler.ValidateToken(refreshTokenValue, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _settings.JwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.JwtSettings.Audience,
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

            var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
            var ownsTransaction = _context.Database.CurrentTransaction == null;

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

            await _context.SaveChangesAsync(cancellationToken);
            if (ownsTransaction)
            {
                await transaction.CommitAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            return TypedResult<RefreshFailureReason, Credentials>.Success(credentials);
        }
        catch (Exception)
        {
            return TypedResult<RefreshFailureReason, Credentials>.Failure(RefreshFailureReason.InvalidRefreshToken, "Invalid refresh token.");
        }
    }

    public async Task<TypedResult<SignInFailureReason, Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;

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
        var user = await _context.DomainUsers.SingleAsync(u => u.Email == Email.Create(email), cancellationToken);
        if (user == null)
        {
            return TypedResult<SignInFailureReason, Credentials>.Failure(SignInFailureReason.UserNotFound, "User not found.");
        }

        var credentials = GenerateCredentials(identityUser, user);
        var refreshTokenEntity = new RefreshTokenEntity(credentials.RefreshToken.Id, user.Id, credentials.RefreshToken.ExpiresAt);
        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync(cancellationToken);
        if (ownsTransaction)
        {
            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }

        return TypedResult<SignInFailureReason, Credentials>.Success(credentials);
    }

    public async Task<TypedResult<SignOutFailureReason, bool>> SignOutAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var userId = _userService.UserId;
        if (userId == null)
        {
            return TypedResult<SignOutFailureReason, bool>.Failure(SignOutFailureReason.Unauthorized, "User is not authenticated.");
        }

        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;

        var refreshToken = ParseRefreshToken(refreshTokenValue);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshToken.Id && rt.UserId == userId, cancellationToken);
        if (refreshTokenEntity == null)
        {
            return TypedResult<SignOutFailureReason, bool>.Failure(SignOutFailureReason.Unauthorized, "Refresh token not found for the user.");
        }
        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
        if (ownsTransaction)
        {
            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }
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
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSettings.AccessTokenSecret));
        var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new System.Security.Claims.Claim("sub", identityUser.Id),
            new System.Security.Claims.Claim("userId", user.Id.ToString()),
            new System.Security.Claims.Claim("refreshTokenId", refreshToken.Id.ToString()),
        };

        var jwtSecurityToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken
        (
            issuer: _settings.JwtSettings.Issuer,
            audience: _settings.JwtSettings.Audience,
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
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSettings.RefreshTokenSecret));
        var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new System.Security.Claims.Claim("sub", identityUser.Id),
            new System.Security.Claims.Claim("userId", user.Id.ToString()),
            new System.Security.Claims.Claim("tokenId", tokenId.ToString()),
        };

        var jwtSecurityToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken
        (
            issuer: _settings.JwtSettings.Issuer,
            audience: _settings.JwtSettings.Audience,
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

        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;
        var refreshToken = ParseRefreshToken(refreshTokenValue);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshToken.Id && rt.UserId == userId, cancellationToken);
        if (refreshTokenEntity == null)
        {
            return TypedResult<InvalidateRefreshTokenFailureReason, bool>.Failure(InvalidateRefreshTokenFailureReason.Unauthorized, "Refresh token not found for the user.");
        }
        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
        if (ownsTransaction)
        {
            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }
        return TypedResult<InvalidateRefreshTokenFailureReason, bool>.Success(true);
    }

    private static RefreshToken ParseRefreshToken(string refreshTokenValue)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
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

    public async Task<TypedResult<RegisterUserFailureReason, IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var identityUser = new MyIdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(identityUser, password);
        if (result.Succeeded)
        {
            return TypedResult<RegisterUserFailureReason, IIdentityUser>.Success(identityUser);
        }
        else
        {
            var failureReason = result.Errors.Any(e => e.Code == "PasswordTooShort" || e.Code == "PasswordRequiresNonAlphanumeric" || e.Code == "PasswordRequiresDigit" || e.Code == "PasswordRequiresUpper" || e.Code == "PasswordRequiresLower")
                ? RegisterUserFailureReason.WeakPassword
                : RegisterUserFailureReason.UnknownError;

            return TypedResult<RegisterUserFailureReason, IIdentityUser>.Failure(failureReason, string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<ResultBase<ResetPasswordFailureReason>> ResetPasswordAsync(string identityUserId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return ResultBase<ResetPasswordFailureReason>.Failure(ResetPasswordFailureReason.UserNotFound, "User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            return ResultBase<ResetPasswordFailureReason>.Success();
        }
        else
        {
            var failureReason = result.Errors.Any(e => e.Code == "PasswordTooShort" || e.Code == "PasswordRequiresNonAlphanumeric" || e.Code == "PasswordRequiresDigit" || e.Code == "PasswordRequiresUpper" || e.Code == "PasswordRequiresLower")
                ? ResetPasswordFailureReason.WeakPassword
                : ResetPasswordFailureReason.UnknownError;

            return ResultBase<ResetPasswordFailureReason>.Failure(failureReason, string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<ResultBase<DeleteUserFailureReason>> DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return ResultBase<DeleteUserFailureReason>.Failure(DeleteUserFailureReason.UserNotFound, "User not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return ResultBase<DeleteUserFailureReason>.Success();
        }
        else
        {
            return ResultBase<DeleteUserFailureReason>.Failure(DeleteUserFailureReason.UnknownError, string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
        }
    }
}