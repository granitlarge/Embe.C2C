using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Money : IComparable<Money>
{
    private Money(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new DomainException(new DomainError<MoneyError>(MoneyError.NegativeAmount));
        }

        Amount = amount;
        Currency = currency;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Money()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public decimal Amount { get; }
    public Currency Currency { get; }

    public static Money Create(decimal amount, Currency currency)
    {
        return new Money(amount, currency);
    }

    public int CompareTo(Money? other)
    {
        if (other != null && Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot compare Money values with different currencies.");
        }

        return Amount.CompareTo(other?.Amount);
    }
}

public enum MoneyError
{
    NegativeAmount,
    CurrencyMismatch
}