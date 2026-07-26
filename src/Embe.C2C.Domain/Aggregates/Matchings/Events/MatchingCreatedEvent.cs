namespace Embe.C2C.Domain.Aggregates.Matchings.Events;

public record MatchingCreatedDomainEvent
(
    Guid LastJudgeUserId,
    Matching Matching
) : DomainEvent;