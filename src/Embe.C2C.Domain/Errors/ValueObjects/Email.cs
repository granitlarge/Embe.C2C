namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class EmailErrors
{
    public static readonly DomainError Invalid = new("email.invalid", "The e-mail provided is invalid.");
}
