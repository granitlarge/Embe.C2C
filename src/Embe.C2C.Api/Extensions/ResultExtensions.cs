using System.Net;
using Embe.C2C.Application.Abstractions;

namespace Embe.C2C.Api.Extensions;

public static class ResultExtensions
{
    private static IResult ToResult<T_FailureReason>(object? value, T_FailureReason failureReason, string? message)
        where T_FailureReason : notnull
    {
        var statusCode = failureReason.ToStatusCode();
        return statusCode switch
        {
            HttpStatusCode.BadRequest => Results.BadRequest(new { success = false, reason = failureReason, message }),
            HttpStatusCode.NotFound => Results.NotFound(),
            HttpStatusCode.Forbidden => Results.Forbid(),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.TooManyRequests => Results.StatusCode((int)HttpStatusCode.TooManyRequests),
            HttpStatusCode.InternalServerError => Results.InternalServerError(new { success = false, reason = failureReason, message }),
            _ => Results.StatusCode((int)statusCode)
        };
    }

    public static IResult ToResult<T_FailureReason>(this ResultBase<T_FailureReason> result)
    where T_FailureReason : notnull
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }
        else
        {
            return ToResult(null, result.Reason!, result.Message);
        }
    }

    public static IResult ToResult<T_FailureReason, T>(this TypedResult<T_FailureReason, T> result)
    where T_FailureReason : notnull
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new { success = true, value = result.Value });
        }
        else
        {
            return ToResult(result.Value, result.Reason!, result.Message);
        }
    }
}