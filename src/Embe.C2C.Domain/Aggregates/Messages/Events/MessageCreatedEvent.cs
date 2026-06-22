namespace Embe.C2C.Domain.Aggregates.Messages.Events;

public record MessageCreatedEvent(Message Message) : DomainEvent;