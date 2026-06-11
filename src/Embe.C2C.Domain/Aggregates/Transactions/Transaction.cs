using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Transactions;

public class Transaction : Aggregate
{
    private Transaction
    (
        Guid accountId,
        Money amount,
        TransactionType type,
        TransactionReason reason,
        DateTimeOffset transactionDate,
        string? note = null
    )
    {

        if (amount.Amount <= 0)
        {
            throw new DomainException(new DomainError<TransactionError>(TransactionError.ZeroOrNegativeAmount));
        }

        if (transactionDate > DateTimeOffset.UtcNow)
        {
            throw new DomainException(new DomainError<TransactionError>(TransactionError.FutureTransactionDate));
        }

        Id = Guid.CreateVersion7();
        AccountId = accountId;
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

    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionReason Reason { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public string? Note { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    internal static Transaction Create
    (
        Guid AccountId,
        Money amount,
        TransactionType type,
        TransactionReason reason,
        DateTimeOffset transactionDate,
        string? note = null
    )
    {
        return new Transaction(AccountId, amount, type, reason, transactionDate, note);
    }
}

public enum TransactionError
{
    ZeroOrNegativeAmount,
    FutureTransactionDate
}