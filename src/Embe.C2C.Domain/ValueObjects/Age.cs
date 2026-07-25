using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record Age : IComparable<Age>
{
    public static Age Create(BirthDate birthDate)
    {
        return new Age(birthDate);
    }

    public static ErrorOr<Age> Create(int age)
    {
        if (age < 0 || age > 120)
        {
            return AgeErrors.AgeOutOfRange.ToValidationErrorOr();
        }

        return new Age(age);
    }

    private Age(BirthDate birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(TimeOnly.MinValue);
        var birthDateTime = birthDate.Value.ToDateTime(TimeOnly.MinValue);
        var difference = today - birthDateTime;
        var age = difference.TotalDays / 365.25;
        Value = (int)age;
    }

    private Age(int age)
    {
        Value = age;
    }

    public int Value { get; }

    public int CompareTo(Age? other)
    {
        return Value.CompareTo(other?.Value);
    }

    public static bool operator <(Age left, Age right) => left.CompareTo(right) < 0;
    public static bool operator >(Age left, Age right) => left.CompareTo(right) > 0;
    public static bool operator <=(Age left, Age right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Age left, Age right) => left.CompareTo(right) >= 0;
}