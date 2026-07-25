using Embe.C2C.Domain.Errors;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record Alias
{
    private Alias(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ErrorOr<Alias> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation(DomainErrors.Empty.Code, DomainErrors.Empty.Message);
        }
        return new Alias(value);
    }
}

public enum AliasError
{
    EmptyOrWhitespace
}