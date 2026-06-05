using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Entities;

public class Transaction
{
    private Transaction
    (
        Money amount,
        TransactionType type,
        TransactionReason reason,
        DateTimeOffset transactionDate,
        string? note = null
    )
    {

        if (amount.Amount <= 0)
        {
            throw new DomainException($"Transaction amount must be greater than zero. Provided amount: {amount}");
        }

        if (transactionDate > DateTimeOffset.UtcNow)
        {
            throw new DomainException($"Transaction date cannot be in the future. Provided date: {transactionDate}");
        }

        Amount = amount;
        Type = type;
        Reason = reason;
        TransactionDate = transactionDate;
        Note = note;
        CreatedAt = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Transaction()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        // For ORM
    }

    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionReason Reason { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public string? Note { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public record ConstructorParameters
    (
        Money Amount,
        TransactionType Type,
        TransactionReason Reason,
        DateTimeOffset TransactionDate,
        string? Note = null
    );

    public static Transaction Create
    (
        Money amount,
        TransactionType type,
        TransactionReason reason,
        DateTimeOffset transactionDate,
        string? note = null
    )
    {
        return new Transaction(amount, type, reason, transactionDate, note);
    }
}