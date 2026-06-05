using Embe.C2C.Domain.Entities;

namespace Embe.C2C.Domain.Aggregates.Accounts.Events;

public record WithdrawalEvent : DomainEvent
{
    public WithdrawalEvent(Account account, Transaction transaction)
    {
        Account = account;
        Transaction = transaction;
    }

    public Account Account { get; init; }
    public Transaction Transaction { get; init; }
}
