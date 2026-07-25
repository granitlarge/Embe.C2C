namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class BirthdateErrors
{
    public static readonly DomainError Invalid = new("birthdate.invalid", "Birthdate is invalid");
}
