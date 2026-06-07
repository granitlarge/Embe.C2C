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