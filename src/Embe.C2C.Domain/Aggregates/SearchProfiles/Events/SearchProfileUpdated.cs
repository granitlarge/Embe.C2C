namespace Embe.C2C.Domain.Aggregates.SearchProfiles.Events;

public record SearchProfileUpdatedDomainEvent(SearchProfile SearchProfile) : DomainEvent;