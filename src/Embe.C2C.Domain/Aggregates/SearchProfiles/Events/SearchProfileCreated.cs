namespace Embe.C2C.Domain.Aggregates.SearchProfiles.Events;

public record SearchProfileCreatedDomainEvent(SearchProfile SearchProfile) : DomainEvent;
