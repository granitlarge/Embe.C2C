using ErrorOr;

namespace Embe.C2C.Application.Abstractions.Services.AuthServices;

public interface IIdentityUser
{
    string Id { get; set; }
    string? Email { get; set; }
}

public interface IAuthService
{
    Task<bool> AccountExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<ErrorOr<Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<ErrorOr<bool>> SignOutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ErrorOr<Credentials>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ErrorOr<bool>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ErrorOr<IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> ResetPasswordAsync(string newPassword, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default);
    Task<string> GeneratePasswordResetLinkAsync(string email, CancellationToken cancellationToken);
}