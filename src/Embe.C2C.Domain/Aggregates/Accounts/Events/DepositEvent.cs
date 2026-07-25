using Embe.C2C.Domain.Aggregates.Transactions;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Domain.Aggregates.Accounts.Events;

public record DepositEvent : DomainEvent
{
    public DepositEvent(Account account, Transaction transaction)
    {
        Account = account;
        Transaction = transaction;
    }

    public Account Account { get; init; }
    public Transaction Transaction { get; init; }
}