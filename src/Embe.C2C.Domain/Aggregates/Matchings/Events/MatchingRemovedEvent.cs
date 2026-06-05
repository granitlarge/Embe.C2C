namespace Embe.C2C.Domain.Aggregates.Matchings.Events;

public record MatchingRemovedEvent(Guid RemoverUserId, Matching Matching) : DomainEvent;