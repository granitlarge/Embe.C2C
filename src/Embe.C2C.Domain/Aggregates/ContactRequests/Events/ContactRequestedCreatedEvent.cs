namespace Embe.C2C.Domain.Aggregates.ContactRequests.Events;

public record ContactRequestCreatedEvent(ContactRequest ContactRequest) : DomainEvent;
