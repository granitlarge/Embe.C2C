using Embe.C2C.Domain.Errors;
using Embe.C2C.Domain.Errors.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.ValueObjects;

public record Money : IComparable<Money>
{
    private Money(decimal amount, Currency currency)
    {
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

    public static ErrorOr<Money> Create(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            return MoneyErrors.Negative.ToValidationErrorOr();
        }

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