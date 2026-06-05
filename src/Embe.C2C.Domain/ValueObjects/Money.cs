using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record Money : IComparable<Money>
{
    private Money(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new DomainException("Amount cannot be negative.");
        }

        Amount = amount;
        Currency = currency;
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
            throw new DomainException("Cannot compare money with different currencies.");
        }

        return Amount.CompareTo(other?.Amount);
    }
}