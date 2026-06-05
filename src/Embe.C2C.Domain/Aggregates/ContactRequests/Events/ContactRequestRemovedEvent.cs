namespace Embe.C2C.Domain.Aggregates.ContactRequests.Events;

public record ContactRequestRemovedEvent(ContactRequest ContactRequest) : DomainEvent;