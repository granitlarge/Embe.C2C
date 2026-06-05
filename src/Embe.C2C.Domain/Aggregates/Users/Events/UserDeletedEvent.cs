namespace Embe.C2C.Domain.Aggregates.Users.Events;

public record UserDeletedEvent(User User) : DomainEvent;