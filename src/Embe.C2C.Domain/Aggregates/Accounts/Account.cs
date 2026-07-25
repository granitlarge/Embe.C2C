using Embe.C2C.Domain.Aggregates.Accounts.Events;
using Embe.C2C.Domain.Aggregates.Transactions;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

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
        Balance = Money.Create(0, currency).Value;
        Open();
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Account()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public static ErrorOr<Account> Open(Guid userId, Currency currency)
    {
        return new Account(userId, currency);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; }
    public Money Balance { get; private set; }
    public Currency Currency => Balance.Currency;
    public bool IsOpen { get; private set; }

    public ErrorOr<Transaction> Withdraw(Money amount)
    {
        if (!IsOpen)
        {
            return DomainErrors.AccountTransactWhileClosed.ToFailureErrorOr();
        }

        if (amount.Currency != Currency)
        {
            return DomainErrors.AccountTransactIncorrectCurrency.ToFailureErrorOr();
        }

        if (amount.Amount > Balance.Amount)
        {
            return DomainErrors.AccountWithdrawExceedsBalance.ToFailureErrorOr();
        }

        if (amount.Amount <= 0)
        {
            return DomainErrors.AccountTransactNonPositiveAmount.ToFailureErrorOr();
        }

        Balance = Money.Create(Balance.Amount - amount.Amount, Balance.Currency).Value;
        var transaction = Transaction.Create
        (
            Id,
            amount,
            TransactionType.Withdrawal,
            TransactionReason.Withdrawal,
            DateTimeOffset.UtcNow
        );

        if (transaction.IsError)
        {
            return transaction.Errors;
        }

        AddDomainEvent(new WithdrawalEvent(this, transaction.Value));
        return transaction;
    }

    public ErrorOr<Transaction> Deposit(Money amount)
    {
        if (!IsOpen)
        {
            return DomainErrors.AccountTransactWhileClosed.ToFailureErrorOr();
        }

        if (amount.Currency != Currency)
        {
            return DomainErrors.AccountTransactIncorrectCurrency.ToFailureErrorOr();
        }

        if (amount.Amount <= 0)
        {
            return DomainErrors.AccountTransactNonPositiveAmount.ToFailureErrorOr();
        }

        Balance = Money.Create(Balance.Amount + amount.Amount, Balance.Currency).Value;

        var transaction = Transaction.Create
        (
            Id,
            amount,
            TransactionType.Deposit,
            TransactionReason.Deposit,
            DateTimeOffset.UtcNow
        );
        if (transaction.IsError)
        {
            return transaction.Errors;
        }
        AddDomainEvent(new DepositEvent(this, transaction.Value));
        return transaction;
    }

    public ErrorOr<Success> Close()
    {
        var errors = new List<Error>();
        if (!IsOpen)
        {
            errors.Add(DomainErrors.AccountCloseAlreadyClosed.ToFailureErrorOr());
        }

        if (Balance.Amount != 0)
        {
            errors.Add(DomainErrors.AccountClosePositiveBalance.ToFailureErrorOr());
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        IsOpen = false;
        AddDomainEvent(new AccountClosedEvent(this));
        return Result.Success;
    }

    public ErrorOr<Success> Open()
    {
        if (IsOpen)
        {
            return DomainErrors.AccountOpenAlreadyOpened.ToFailureErrorOr();
        }

        IsOpen = true;
        AddDomainEvent(new AccountOpenedEvent(this));
        return Result.Success;
    }

    internal ErrorOr<Success> Remove()
    {
        if (IsOpen)
        {
            return DomainErrors.AccountRemoveWhileOpen.ToFailureErrorOr();
        }

        AddDomainEvent(new AccountRemovedEvent(this));
        return Result.Success;
    }
}