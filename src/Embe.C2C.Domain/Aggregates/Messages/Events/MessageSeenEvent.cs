namespace Embe.C2C.Domain.Aggregates.Messages.Events;

public record MessageSeenEvent(Message Message) : DomainEvent;