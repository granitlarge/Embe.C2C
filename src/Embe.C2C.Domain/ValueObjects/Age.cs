using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Age : IComparable<Age>
{
    public Age(BirthDate birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Value.Year;
        if (birthDate.Value > today.AddYears(-age))
        {
            age--;
        }

        if (age < 0)
        {
            throw new DomainException("Birth date cannot be in the future.");
        }

        Value = age;
    }

    public Age(int age)
    {
        if (age < 0)
        {
            throw new DomainException("Age cannot be negative.");
        }

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