namespace Embe.C2C.Domain.Aggregates.Messages.Events;

public record MessageEditedEvent(Message Message) : DomainEvent;