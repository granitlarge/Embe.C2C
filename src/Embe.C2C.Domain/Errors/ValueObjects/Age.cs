namespace Embe.C2C.Domain.Errors.ValueObjects;

public static class AgeErrors
{
    public static readonly DomainError AgeOutOfRange = new("age.out_of_range", "Age must be between 0 and 120");
}