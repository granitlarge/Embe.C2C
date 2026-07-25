using Embe.C2C.Domain.Errors;
using ErrorOr;

namespace Embe.C2C.Application.Errors;

public record ApplicationError(string Code, string Message);

public enum ApplicationErrorType
{
    DomainRule = 0,
    Validation = 1,
    Forbidden = 2,
    Unexpected = 3,
    NotFound = 4,
}

public static class ApplicationErrorExtensions
{
    public static Error ToForbiddenErrorOr(this ApplicationError applicationError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.Forbidden, applicationError.Code, applicationError.Message, metadata);
    }

    public static Error ToNotFoundErrorOr(this ApplicationError applicationError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.NotFound, applicationError.Code, applicationError.Message, metadata);
    }

    public static Error ToValidationErrorOr(this ApplicationError applicationError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.Validation, applicationError.Code, applicationError.Message, metadata);
    }

    public static Error ToUnexpectedErrorOr(this ApplicationError applicationError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.Unexpected, applicationError.Code, applicationError.Message, metadata);
    }

    public static IEnumerable<Error> ToValidationErrorOr(this IEnumerable<ApplicationError> applicationErrors)
    {
        return applicationErrors.Select(e => e.ToValidationErrorOr());
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