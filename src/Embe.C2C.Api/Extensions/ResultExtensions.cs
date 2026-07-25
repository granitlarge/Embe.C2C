using Embe.C2C.Application.Errors;
using ErrorOr;

namespace Embe.C2C.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToResult<T>(this ErrorOr<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new { success = true, value = result.Value });
        }
        else
        {
            return result.FirstError switch
            {
                Error error when error.NumericType == (int)ApplicationErrorType.Validation => Results.BadRequest(new { success = false, errors = result.Errors }),
                Error error when error.NumericType == (int)ApplicationErrorType.NotFound => Results.NotFound(new { success = false, errors = result.Errors }),
                Error error when error.NumericType == (int)ApplicationErrorType.Forbidden => Results.Forbid(),
                Error error when error.NumericType == (int)ApplicationErrorType.Unauthorized => Results.Unauthorized(),
                _ => Results.InternalServerError(new { success = false, errors = result.Errors })
            };
        }
    }
}