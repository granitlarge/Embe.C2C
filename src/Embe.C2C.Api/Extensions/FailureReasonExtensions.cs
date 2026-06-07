using System.Net;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Commands.Users.Handlers;

namespace Embe.C2C.Api.Extensions;

public static class FailureReasonExtensions
{
    public static HttpStatusCode ToStatusCode(this FailureReason reason)
    {
        return reason switch
        {
            FailureReason.NotFound => HttpStatusCode.NotFound,
            FailureReason.Forbidden => HttpStatusCode.Forbidden,
            FailureReason.DomainError => HttpStatusCode.BadRequest,
            FailureReason.Unknown => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static HttpStatusCode ToStatusCode(this SignInFailureReason reason)
    {
        return reason switch
        {
            SignInFailureReason.InvalidCredentials => HttpStatusCode.BadRequest,
            SignInFailureReason.UserNotFound => HttpStatusCode.NotFound,
            SignInFailureReason.UserNotConfirmed => HttpStatusCode.Forbidden,
            SignInFailureReason.TooManyAttempts => HttpStatusCode.TooManyRequests,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static HttpStatusCode ToStatusCode(this SignOutFailureReason reason)
    {
        return reason switch
        {
            SignOutFailureReason.Unauthorized => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static HttpStatusCode ToStatusCode(this RefreshFailureReason reason)
    {
        return reason switch
        {
            RefreshFailureReason.InvalidRefreshToken => HttpStatusCode.BadRequest,
            RefreshFailureReason.ExpiredRefreshToken => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static HttpStatusCode ToStatusCode(this InvalidateRefreshTokenFailureReason reason)
    {
        return reason switch
        {
            InvalidateRefreshTokenFailureReason.Unauthorized => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static HttpStatusCode ToStatusCode(this RegisterUserFailureReason reason)
    {
        return reason switch
        {
            RegisterUserFailureReason.EmailAlreadyExists => HttpStatusCode.BadRequest,
            RegisterUserFailureReason.DomainError => HttpStatusCode.BadRequest,
            RegisterUserFailureReason.WeakPassword => HttpStatusCode.BadRequest,
            RegisterUserFailureReason.Unknown => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static HttpStatusCode ToStatusCode(this object reason)
    {
        return reason switch
        {
            FailureReason r => r.ToStatusCode(),
            SignInFailureReason r => r.ToStatusCode(),
            SignOutFailureReason r => r.ToStatusCode(),
            RefreshFailureReason r => r.ToStatusCode(),
            InvalidateRefreshTokenFailureReason r => r.ToStatusCode(),
            RegisterUserFailureReason r => r.ToStatusCode(),
            _ => throw new NotImplementedException()
        };
    }
}