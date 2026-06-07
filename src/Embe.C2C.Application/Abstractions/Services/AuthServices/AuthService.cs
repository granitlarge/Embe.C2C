using Embe.C2C.Application.Commands.Users.Handlers;

namespace Embe.C2C.Application.Abstractions.Services.AuthServices;

public interface IIdentityUser
{
    string Id { get; set; }
    string? Email { get; set; }
}

public interface IAuthService
{
    Task<bool> AccountExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<TypedResult<SignInFailureReason, Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<TypedResult<SignOutFailureReason, bool>> SignOutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<TypedResult<RefreshFailureReason, Credentials>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<TypedResult<InvalidateRefreshTokenFailureReason, bool>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<TypedResult<RegisterUserFailureReason, IIdentityUser>> RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<ResultBase<ResetPasswordFailureReason>> ResetPasswordAsync(string identityUserId, string newPassword, CancellationToken cancellationToken = default);
    Task<ResultBase<DeleteUserFailureReason>> DeleteUserAsync(string identityUserId, CancellationToken cancellationToken = default);
}

public enum InvalidateRefreshTokenFailureReason
{
    Unauthorized = 1
}

public enum SignInFailureReason
{
    InvalidCredentials = 0,
    UserNotFound = 1,
    UserNotConfirmed = 2,
    TooManyAttempts = 3,
}

public enum SignOutFailureReason
{
    Unauthorized = 1
}

public enum RefreshFailureReason
{
    InvalidRefreshToken = 0,
    ExpiredRefreshToken = 1,
}

public enum DeleteUserFailureReason
{
    UserNotFound,
    UnknownError
}

public enum ResetPasswordFailureReason
{
    UserNotFound,
    WeakPassword,
    UnknownError
}