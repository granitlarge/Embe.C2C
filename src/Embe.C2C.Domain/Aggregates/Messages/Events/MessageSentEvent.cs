namespace Embe.C2C.Domain.Aggregates.Messages.Events;

public record MessageSentEvent(Message Message) : DomainEvent;