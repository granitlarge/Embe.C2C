namespace Embe.C2C.Domain.Aggregates.SearchProfiles.Events;

public record SearchProfileDescriptionChangedDomainEvent(Guid SearchProfileId, string NewDescription) : DomainEvent;