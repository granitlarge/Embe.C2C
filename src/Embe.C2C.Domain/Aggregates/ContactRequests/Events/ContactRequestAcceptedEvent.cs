namespace Embe.C2C.Domain.Aggregates.ContactRequests.Events;

public record ContactRequestAcceptedEvent(ContactRequest ContactRequest) : DomainEvent;
