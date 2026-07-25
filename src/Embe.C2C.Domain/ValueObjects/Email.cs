using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record Email
{
    private Email
    (
        string value
    )
    {
        Value = value;
    }

    public string Value { get; init; }

    public static ErrorOr<Email> Create(string value)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
        }
        catch (Exception)
        {
            return EmailErrors.Invalid.ToValidationErrorOr();
        }
        return new Email(value);
    }

}