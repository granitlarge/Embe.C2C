namespace Embe.C2C.Domain.Aggregates.Blockings.Events;

public record BlockingCreatedEvent(Blocking Blocking) : DomainEvent;