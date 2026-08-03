using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Errors;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Infrastructure.Ef.Entities;
using Embe.C2C.Infrastructure.Extensions;
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
    private static readonly TimeSpan _resetPasswordTokenLifetime = TimeSpan.FromMinutes(15);

    public async Task<ErrorOr<Credentials>> RefreshAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Jwt.RefreshTokenSecret));

        try
        {
            var principal = tokenHandler.ValidateToken(refreshTokenValue, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _settings.Jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            if (validatedToken is null)
            {
                return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
            }

            var refreshTokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
            if (refreshTokenIdClaim == null || !Guid.TryParse(refreshTokenIdClaim.Value, out var refreshTokenId))
            {
                return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
            }

            var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
            var ownsTransaction = _context.Database.CurrentTransaction == null;

            var refreshTokenEntity = await _context.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(rt => rt.Id == refreshTokenId, cancellationToken);
            if (refreshTokenEntity == null)
            {
                return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
            }

            var refreshTokenHasExpired = refreshTokenEntity.ExpiresAt < DateTimeOffset.UtcNow;
            if (refreshTokenHasExpired)
            {
                return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
            }

            var user = await _context.DomainUsers.AsNoTracking().SingleOrDefaultAsync(u => u.Id == refreshTokenEntity.UserId, cancellationToken);
            if (user == null)
            {
                return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
            }

            var identityUser = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == user.Email.Value, cancellationToken);
            if (identityUser == null)
            {
                return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
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
            return ApplicationErrors.InvalidRefreshToken.ToValidationErrorOr();
        }
    }

    public async Task<ErrorOr<Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(cancellationToken);
        var ownsTransaction = _context.Database.CurrentTransaction == null;

        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser == null)
        {
            return ApplicationErrors.NoUserWithSuppliedEmail.ToValidationErrorOr();
        }

        var isLockedOut = await _userManager.IsLockedOutAsync(identityUser);

        if (isLockedOut)
        {
            return ApplicationErrors.LockedOut.ToValidationErrorOr();
        }

        var signInResult = await _userManager.CheckPasswordAsync(identityUser, password);
        if (!signInResult)
        {
            await _userManager.AccessFailedAsync(identityUser);
            return ApplicationErrors.InvalidCredentials.ToValidationErrorOr();
        }
        else
        {
            await _userManager.ResetAccessFailedCountAsync(identityUser);
        }

        var domainEmail = Email.Create(email);
        if (domainEmail.IsError)
        {
            return domainEmail.Errors;
        }

        var user = await _context.DomainUsers.SingleAsync(u => u.Email == domainEmail.Value, cancellationToken);
        if (identityUser == null)
        {
            return ApplicationErrors.NoUserWithSuppliedEmail.ToValidationErrorOr();
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
            return ApplicationErrors.Unauthorized.ToUnauthorizedErrorOr();
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
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Jwt.AccessTokenSecret));
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
            issuer: _settings.Jwt.Issuer,
            audience: _settings.Jwt.Audience,
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
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Jwt.RefreshTokenSecret));
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
            issuer: _settings.Jwt.Issuer,
            audience: _settings.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_refreshTokenLifetime),
            signingCredentials: credentials
        );

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return new RefreshToken(tokenId, token, DateTimeOffset.UtcNow.Add(_refreshTokenLifetime));
    }

    private string GenerateResetPasswordToken(IdentityUser identityUser, User user)
    {
        var tokenId = Guid.CreateVersion7();
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Jwt.ResetPasswordTokenSecret));
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
            issuer: _settings.Jwt.Issuer,
            audience: _settings.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_resetPasswordTokenLifetime),
            signingCredentials: credentials
        );

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    public async Task<ErrorOr<bool>> InvalidateRefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken)
    {
        var userId = _userService.UserId;
        if (userId == null)
        {
            return ApplicationErrors.Unauthorized.ToUnauthorizedErrorOr();
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

    public async Task<ErrorOr<IIdentityUser>> RegisterUserAsync(Guid userId, string email, string password, CancellationToken cancellationToken = default)
    {
        var identityUser = new MyIdentityUser { UserName = email, Email = email, UserId = userId };
        var result = await _userManager.CreateAsync(identityUser, password);
        if (result.Succeeded)
        {
            return identityUser;
        }
        else
        {
            return ErrorOrFactory.From<IIdentityUser>(result.Errors.ToApplicationErrors().ToValidationErrorOr());
        }
    }

    public async Task<ErrorOr<Success>> ResetPasswordAsync
    (
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _userService.UserId ?? throw new InvalidOperationException("No authenticated user");
        var identityUserId = await _context.Users.Where(u => u.UserId == userId)
            .Select(u => u.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (identityUserId == null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            await ClearRefreshTokens(userId, cancellationToken);
            await _userManager.ResetAccessFailedCountAsync(user);
            return Result.Success;
        }
        else
        {
            return ErrorOrFactory.From<Success>(result.Errors.ToApplicationErrors().ToValidationErrorOr());
        }
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Result.Success;
        }
        else
        {
            return ErrorOrFactory.From<Success>(result.Errors.ToApplicationErrors().ToValidationErrorOr());
        }
    }

    private async Task ClearRefreshTokens(Guid userId, CancellationToken cancellationToken)
    {
        var refreshTokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync(cancellationToken);
        _context.RefreshTokens.RemoveRange(refreshTokens);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static Uri BuildResetPasswordUrl(string resetPasswordUrl, string token)
    {
        var uriBuilder = new UriBuilder(resetPasswordUrl);
        var queryHasParameters = uriBuilder.Query.Split("&").Any(e => e != "");
        var tokenQueryParameter = $"token={UrlEncoder.Create().Encode(token)}";
        uriBuilder.Query += queryHasParameters ? "&" : "" + tokenQueryParameter;
        return uriBuilder.Uri;
    }

    public async Task<string> GeneratePasswordResetLinkAsync(string email, CancellationToken cancellationToken)
    {
        var identityUser = await _userManager.FindByEmailAsync(email) ?? throw new InvalidOperationException("identity user does not exist");
        var user = await _context.DomainUsers.SingleOrDefaultAsync(du => du.Email == Email.Create(email).Value, cancellationToken: cancellationToken) ?? throw new InvalidOperationException("user does not exist");
        var token = GenerateResetPasswordToken(identityUser, user);

        var resetPasswordUrl = BuildResetPasswordUrl($"{_settings.Site.Url}/public/reset-password", token);
        return resetPasswordUrl.ToString();
    }
}