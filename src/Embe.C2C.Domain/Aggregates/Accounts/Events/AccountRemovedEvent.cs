namespace Embe.C2C.Domain.Aggregates.Accounts.Events;

public record AccountRemovedEvent(Account Account) : DomainEvent;