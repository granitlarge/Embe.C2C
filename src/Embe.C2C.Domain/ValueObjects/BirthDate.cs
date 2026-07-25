using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record BirthDate
{
    public static ErrorOr<BirthDate> Create
    (
        DateOnly value
    )
    {
        if (value < DateOnly.FromDateTime(new DateTime(1900, 1, 1)) || value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return BirthdateErrors.Invalid.ToValidationErrorOr();
        }

        return new BirthDate(value);
    }

    private BirthDate(DateOnly value)
    {
        Value = value;
    }

    public DateOnly Value { get; }
}