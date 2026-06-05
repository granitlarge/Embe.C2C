namespace Embe.C2C.Domain.Aggregates.Matchings.Events;

public record MatchingCreatedEvent
(
    Guid LastJudgeUserId,
    Matching Matching
) : DomainEvent;