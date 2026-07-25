using System.Security.Claims;
using System.Text;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Embe.C2C.Infrastructure.Identity;

public class AuthService
(
    Settings settings,
    Ef.Contexts.C2CContext context,
    IAuthenticatedUserService userService,
    UserManager<MyIdentityUser> userManager
) : IAuthService
{
    private readonly Ef.Contexts.C2CContext _context = context;
    private readonly UserManager<MyIdentityUser> _userManager = userManager;
    private readonly IAuthenticatedUserService _userService = userService;
    private readonly Settings _settings = settings;
    private static readonly TimeSpan _accessTokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<ErrorOr<Credentials>> RefreshAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
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

            if (validatedToken is null)
            {
                return Error.Failure("invalid_refresh_token", "Invalid refresh token.");
            }

            var refreshTokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
            if (refreshTokenIdClaim == null || !Guid.TryParse(refreshTokenIdClaim.Value, out var refreshTokenId))
            {
                return Error.Failure("invalid_refresh_token", "Invalid refresh token.");
            }

            var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
            var ownsTransaction = _context.Database.CurrentTransaction == null;

            var refreshTokenEntity = await _context.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(rt => rt.Id == refreshTokenId, cancellationToken);
            if (refreshTokenEntity == null)
            {
                return Error.Failure("invalid_refresh_token", "Invalid refresh token.");
            }

            var refreshTokenHasExpired = refreshTokenEntity.ExpiresAt < DateTimeOffset.UtcNow;
            if (refreshTokenHasExpired)
            {
                return Error.Failure("invalid_refresh_token", "Refresh token has expired.");
            }

            var user = await _context.DomainUsers.AsNoTracking().SingleOrDefaultAsync(u => u.Id == refreshTokenEntity.UserId, cancellationToken);
            if (user == null)
            {
                return Error.Failure("invalid_refresh_token", "Invalid refresh token.");
            }

            var identityUser = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == user.Email.Value, cancellationToken);
            if (identityUser == null)
            {
                return Error.Failure("invalid_refresh_token", "Invalid refresh token.");
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

            return credentials;
        }
        catch (Exception)
        {
            return Error.Failure("invalid_refresh_token", "Invalid refresh token.");
        }
    }

    public async Task<ErrorOr<Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;

        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser == null)
        {
            return Error.NotFound("invalid_email", "No user found with the provided email.");
        }

        var signInResult = await _userManager.CheckPasswordAsync(identityUser, password);
        if (!signInResult)
        {
            return Error.Failure("invalid_credentials", "Invalid email or password.");
        }

        var domainEmail = Email.Create(email);
        if (domainEmail.IsError)
        {
            return domainEmail.Errors;
        }

        var user = await _context.DomainUsers.SingleAsync(u => u.Email == domainEmail.Value, cancellationToken);
        if (identityUser == null)
        {
            return Error.NotFound("invalid_email", "No user found with the provided email.");
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

        return credentials;
    }

    public async Task<ErrorOr<bool>> SignOutAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var userId = _userService.UserId;
        if (userId == null)
        {
            return Error.Failure("unauthorized", "User is not authenticated.");
        }

        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;

        var refreshToken = ParseRefreshToken(refreshTokenValue);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshToken.Id && rt.UserId == userId, cancellationToken);
        if (refreshTokenEntity == null)
        {
            return true;
        }

        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
        if (ownsTransaction)
        {
            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }
        return true;
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
            new Claim("sub", user.Id.ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("identityUserId", identityUser.Id),
            new Claim("refreshTokenId", refreshToken.Id.ToString()),
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
            new Claim("sub", user.Id.ToString()),
            new Claim("identityUserId", identityUser.Id),
            new Claim("userId", user.Id.ToString()),
            new Claim("tokenId", tokenId.ToString()),
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

    public async Task<ErrorOr<bool>> InvalidateRefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken)
    {
        var userId = _userService.UserId;
        if (userId == null)
        {
            return Error.Unauthorized("unauthorized", "User is not authenticated.");
        }
        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;
        var refreshToken = ParseRefreshToken(refreshTokenValue);
        var refreshTokenEntity = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Id == refreshToken.Id && rt.UserId == userId, cancellationToken);
        if (refreshTokenEntity == null)
        {
            return true;
        }
        _context.RefreshTokens.Remove(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
        if (ownsTransaction)
        {
            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }
        return true;
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

    public async Task<ErrorOr<IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var identityUser = new MyIdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(identityUser, password);
        if (result.Succeeded)
        {
            return identityUser;
        }
        else
        {
            return result.Errors.Select(e => Error.Failure("registration_failed", e.Description)).ToList();
        }
    }

    public async Task<ErrorOr<Success>> ResetPasswordAsync(string identityUserId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return Error.Failure("user_not_found", "User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            return ErrorOr.Result.Success;
        }
        else
        {
            return result.Errors.Select(e => Error.Failure("reset_password_failed", e.Description)).ToList();
        }
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return Error.Failure("user_not_found", "User not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return ErrorOr.Result.Success;
        }
        else
        {
            return result.Errors.Select(e => Error.Failure("delete_user_failed", e.Description)).ToList();
        }
    }
}