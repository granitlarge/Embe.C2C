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
    public static Error ToValidationErrorOr(this ApplicationError domainErrorNew, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.Validation, domainErrorNew.Code, domainErrorNew.Message, metadata);
    }

    public static Error ToDomainRuleErrorOr(this ApplicationError domainError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.DomainRule, domainError.Code, domainError.Message, metadata);
    }

    public static Error ToUnexpectedErrorOr(this ApplicationError domainError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)ApplicationErrorType.Unexpected, domainError.Code, domainError.Message, metadata);
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

    public static ErrorOr<T> ToApplicationError<T>(this ErrorOr<T> error)
    {
        if (error.IsError)
        {
            return error.ToApplicationError();
        }
        return error;
    }
}