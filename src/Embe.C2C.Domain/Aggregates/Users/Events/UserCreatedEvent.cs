namespace Embe.C2C.Domain.Aggregates.Users.Events;

public record UserCreatedEvent(User User) : DomainEvent;