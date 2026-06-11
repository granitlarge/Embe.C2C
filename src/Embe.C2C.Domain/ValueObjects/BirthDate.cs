using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record BirthDate
{
    public BirthDate(DateOnly value)
    {
        if (value < DateOnly.FromDateTime(new DateTime(1900, 1, 1)) || value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new DomainException(new DomainError<BirthDateError>(BirthDateError.InvalidBirthDate));
        }
        Value = value;
    }

    public DateOnly Value { get; }
}

public enum BirthDateError
{
    InvalidBirthDate
}