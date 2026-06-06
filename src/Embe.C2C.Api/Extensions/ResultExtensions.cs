using Embe.C2C.Application.Abstractions;

namespace Embe.C2C.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToResult<T_FailureReason>(this ResultBase<T_FailureReason> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok();
        }
        else
        {
            return Results.BadRequest(new { reason = result.Reason, message = result.Message });
        }
    }

    public static IResult ToResult<T_FailureReason, T>(this TypedResult<T_FailureReason, T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }
        else
        {
            return Results.BadRequest(new { reason = result.Reason, message = result.Message });
        }
    }
}