namespace Embe.C2C.Domain.Aggregates.Accounts.Events;

public record AccountOpenedEvent(Account Account) : DomainEvent;
