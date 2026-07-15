namespace Embe.C2C.Domain.Aggregates.Users.Events;

public record UserImageRemovedEvent(Entities.Image Image) : DomainEvent;