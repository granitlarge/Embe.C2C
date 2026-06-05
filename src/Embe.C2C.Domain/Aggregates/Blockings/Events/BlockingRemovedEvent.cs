namespace Embe.C2C.Domain.Aggregates.Blockings.Events;

public record BlockingRemovedEvent(Blocking Blocking) : DomainEvent;