namespace Embe.C2C.Domain.Aggregates.Users.Events;

public record UserFileRemovedEvent(Entities.File File) : DomainEvent;