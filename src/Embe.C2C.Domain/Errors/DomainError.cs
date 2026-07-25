using ErrorOr;

namespace Embe.C2C.Domain.Errors;

public record DomainError(string Code, string Message);

public enum DomainErrorType
{
    Rule = 0,
    Validation = 1,
    Unexpected = 2,
}

public static class DomainErrors
{
    public static readonly DomainError Empty = new("empty", "A value must be provided, but none was.");
    public static readonly DomainError UserSame = new("user.same", "The same user cannot be specified as both arguments.");
    public static readonly DomainError Forbidden = new("forbidden", "The actor that attempted to perform this operation is forbidden to do so.");
}

public static class DomainErrorExtensions
{
    public static Error ToValidationErrorOr(this DomainError domainErrorNew, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)DomainErrorType.Validation, domainErrorNew.Code, domainErrorNew.Message, metadata);
    }

    public static Error ToRuleErrorOr(this DomainError domainError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)DomainErrorType.Rule, domainError.Code, domainError.Message, metadata);
    }

    public static Error ToUnexpectedErrorOr(this DomainError domainError, Dictionary<string, object>? metadata = null)
    {
        return Error.Custom((int)DomainErrorType.Unexpected, domainError.Code, domainError.Message, metadata);
    }
}