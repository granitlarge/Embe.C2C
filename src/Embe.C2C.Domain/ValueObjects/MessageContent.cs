using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record MessageContent
{
    private MessageContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Message content cannot be null or whitespace.");
        }
        Value = value;
    }

    public string Value { get; }
    public static MessageContent Create(string value)
    {
        return new MessageContent(value);
    }
}