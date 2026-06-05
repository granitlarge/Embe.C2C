using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record UserName
{
    private UserName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("User name cannot be null or whitespace.");
        }
        Value = value;
    }

    public string Value { get; }

    public static UserName Create(string value)
    {
        return new UserName(value);
    }
}