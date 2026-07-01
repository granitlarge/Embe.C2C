using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Alias
{
    private Alias(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(new DomainError<AliasError>(AliasError.EmptyOrWhitespace));
        }
        Value = value;
    }

    public string Value { get; }

    public static Alias Create(string value)
    {
        return new Alias(value);
    }
}

public enum AliasError
{
    EmptyOrWhitespace
}