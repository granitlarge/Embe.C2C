using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record MessageContent
{
    private MessageContent(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ErrorOr<MessageContent> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.Empty.ToValidationErrorOr();
        }
        return new MessageContent(value);
    }
}