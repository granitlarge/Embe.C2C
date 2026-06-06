namespace Embe.C2C.Application.Abstractions.Services.AuthServices;

public interface IAuthService
{
    Task<bool> AccountExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<TypedResult<SignInFailureReason, Credentials>> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<TypedResult<SignOutFailureReason, bool>> SignOutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<TypedResult<RefreshFailureReason, Credentials>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<TypedResult<InvalidateRefreshTokenFailureReason, bool>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}

public enum InvalidateRefreshTokenFailureReason
{
    UnknownError = 0,
    Unauthorized = 1
}

public enum SignInFailureReason
{
    InvalidCredentials = 0,
    UserNotFound = 1,
    UserNotConfirmed = 2,
    TooManyAttempts = 3,
    UnknownError = 4
}

public enum SignOutFailureReason
{
    UnknownError = 0,
    Unauthorized = 1
}

public enum RefreshFailureReason
{
    InvalidRefreshToken = 0,
    ExpiredRefreshToken = 1,
    UnknownError = 5
}