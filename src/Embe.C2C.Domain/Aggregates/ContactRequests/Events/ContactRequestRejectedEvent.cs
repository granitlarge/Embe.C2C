namespace Embe.C2C.Domain.Aggregates.ContactRequests.Events;

public record ContactRequestRejectedEvent(ContactRequest ContactRequest) : DomainEvent;