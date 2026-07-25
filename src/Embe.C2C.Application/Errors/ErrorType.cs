using Embe.C2C.Domain.Errors;
using ErrorOr;

namespace Embe.C2C.Application.Errors;

public record ApplicationError(string Code, string Message);

public enum ApplicationErrorType
{
    DomainRule = 0,
    Validation = 1,
    Forbidden = 2,
    Unexpected = 3
}

public static class ApplicationErrorExtensions
{
    public static Error ToForbiddenErrorOr(this ApplicationError applicationError, Dictionary<string, object>? metadata = null)
    {
        return Error.Forbidden(applicationError.Code, applicationError.Message, metadata);
    }

    public static Error ToNotFoundErrorOr(this ApplicationError applicationError, Dictionary<string, object>? metadata = null)
    {
        return Error.NotFound(applicationError.Code, applicationError.Message, metadata);
    }
}

public static class DomainErrorExtensions
{
    public static Error ToApplicationError(this Error error)
    {
        var applicationErrorType = error.NumericType switch
        {
            (int)DomainErrorType.Rule => ApplicationErrorType.DomainRule,
            (int)DomainErrorType.Validation => ApplicationErrorType.Validation,
            (int)DomainErrorType.Unexpected => ApplicationErrorType.Unexpected,
            _ => throw new NotImplementedException()

        };
        return Error.Custom((int)applicationErrorType, error.Code, error.Description);
    }

    public static IEnumerable<Error> ToApplicationError(this IEnumerable<Error> errors)
    {
        return errors.Select(e => e.ToApplicationError());
    }

    public static ErrorOr<T> ToApplicationError<T>(this ErrorOr<T> error)
    {
        return error.ToApplicationError();
    }

}