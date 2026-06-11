using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Email
{
    public static readonly Email Anonymized = new("anonymized@example.com");

    private Email
    (
        string value
    )
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            Value = addr.Address;
        }
        catch (Exception)
        {
            throw new DomainException(new DomainError<EmailError>(EmailError.InvalidFormat));
        }
    }

    public string Value { get; init; }

    public static Email Create(string value)
    {
        return new Email(value);
    }
}

public enum EmailError
{
    InvalidFormat
}