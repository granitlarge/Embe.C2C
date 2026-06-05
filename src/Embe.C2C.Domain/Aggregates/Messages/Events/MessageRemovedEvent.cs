namespace Embe.C2C.Domain.Aggregates.Messages.Events;

public record MessageRemovedEvent(Message Message) : DomainEvent;