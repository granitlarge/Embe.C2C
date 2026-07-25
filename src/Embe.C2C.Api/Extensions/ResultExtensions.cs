using System.Net;
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
                Error error when error.Type == ErrorType.Validation => Results.BadRequest(new { success = false, errors = result.Errors }),
                Error error when error.Type == ErrorType.NotFound => Results.NotFound(new { success = false, errors = result.Errors }),
                Error error when error.Type == ErrorType.Conflict => Results.Conflict(new { success = false, errors = result.Errors }),
                Error error when error.Type == ErrorType.Forbidden => Results.Forbid(),
                Error error when error.Type == ErrorType.Unauthorized => Results.Unauthorized(),
                Error error when error.Type == ErrorType.Unexpected => Results.InternalServerError(new { success = false, errors = result.Errors }),
                Error error when error.Type == ErrorType.Failure => Results.InternalServerError(new { success = false, errors = result.Errors }),
                _ => Results.StatusCode((int)HttpStatusCode.InternalServerError)
            };
        }
    }
}