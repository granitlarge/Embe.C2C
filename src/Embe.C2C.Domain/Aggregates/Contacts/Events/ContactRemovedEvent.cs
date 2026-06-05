namespace Embe.C2C.Domain.Aggregates.Contacts.Events;

public record ContactRemovedEvent(Guid RemoverUserId, Contact Contact) : DomainEvent;