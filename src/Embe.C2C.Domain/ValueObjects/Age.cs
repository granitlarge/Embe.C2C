using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Age : IComparable<Age>
{
    public Age(BirthDate birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(TimeOnly.MinValue);
        var birthDateTime = birthDate.Value.ToDateTime(TimeOnly.MinValue);
        var difference = today - birthDateTime;
        var age = difference.TotalDays / 365.25;
        if (age < 0)
        {
            throw new DomainException(new DomainError<AgeError>(AgeError.FutureBirthDate));
        }
        Value = (int)age;
    }

    public Age(int age)
    {
        if (age < 0)
        {
            throw new DomainException(new DomainError<AgeError>(AgeError.NegativeAge));
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

public enum AgeError
{
    NegativeAge,
    FutureBirthDate
}