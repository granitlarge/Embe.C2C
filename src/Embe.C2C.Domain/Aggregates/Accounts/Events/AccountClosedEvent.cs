namespace Embe.C2C.Domain.Aggregates.Accounts.Events;

public record AccountClosedEvent(Account Account) : DomainEvent;