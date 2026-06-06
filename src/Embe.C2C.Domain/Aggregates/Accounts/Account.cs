using Embe.C2C.Domain.Aggregates.Accounts.Events;
using Embe.C2C.Domain.Aggregates.Transactions;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Accounts;

public class Account : Aggregate
{
    private Account
    (
        Guid userId,
        Currency currency
    )
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Balance = Money.Create(0, currency);
        Open();
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Account()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public static Account Open(Guid userId, Currency currency)
    {
        return new Account(userId, currency);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; }
    public Money Balance { get; private set; }
    public Currency Currency => Balance.Currency;
    public bool IsOpen { get; private set; }

    public Transaction Withdraw(Money amount)
    {
        if (!IsOpen)
        {
            throw new DomainException("Account is closed.");
        }

        if (amount.Currency != Currency)
        {
            throw new DomainException($"Currency mismatch. Account currency: {Currency}, Request currency: {amount.Currency}");
        }

        if (amount.Amount > Balance.Amount)
        {
            throw new DomainException($"Insufficient funds. Account balance: {Balance}, Requested amount: {amount}");
        }

        if (amount.Amount <= 0)
        {
            throw new DomainException($"Withdrawal amount must be greater than zero. Requested amount: {amount}");
        }

        Balance = Money.Create(Balance.Amount - amount.Amount, Balance.Currency);
        var transaction = Transaction.Create
        (
            Id,
            amount,
            TransactionType.Withdrawal,
            TransactionReason.Withdrawal,
            DateTimeOffset.UtcNow
        );

        AddDomainEvent(new WithdrawalEvent(this, transaction));
        return transaction;
    }

    public Transaction Deposit(Money amount)
    {
        if (!IsOpen)
        {
            throw new DomainException("Account is closed.");
        }

        if (amount.Currency != Currency)
        {
            throw new DomainException($"Currency mismatch. Account currency: {Currency}, Request currency: {amount.Currency}");
        }

        if (amount.Amount <= 0)
        {
            throw new DomainException($"Deposit amount must be greater than zero. Requested amount: {amount}");
        }

        Balance = Money.Create(Balance.Amount + amount.Amount, Balance.Currency);
        var transaction = Transaction.Create
        (
            Id,
            amount,
            TransactionType.Deposit,
            TransactionReason.Deposit,
            DateTimeOffset.UtcNow
        );

        AddDomainEvent(new DepositEvent(this, transaction));
        return transaction;
    }

    public void Close()
    {
        if (!IsOpen)
        {
            throw new DomainException("Account is already closed.");
        }

        if (Balance.Amount != 0)
        {
            throw new DomainException("Account balance must be zero to close the account.");
        }

        IsOpen = false;
        AddDomainEvent(new AccountClosedEvent(this));
    }

    public void Open()
    {
        if (IsOpen)
        {
            throw new DomainException("Account is already open.");
        }

        IsOpen = true;
        AddDomainEvent(new AccountOpenedEvent(this));
    }

    public void Remove()
    {
        if (IsOpen)
        {
            throw new DomainException("Account must be closed before it can be removed.");
        }
        AddDomainEvent(new AccountRemovedEvent(this));
    }
}