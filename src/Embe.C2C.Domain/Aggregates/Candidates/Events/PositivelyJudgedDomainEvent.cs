namespace Embe.C2C.Domain.Aggregates.Candidates.Events;

public record PositivelyJudgedDomainEvent(Candidate Candidate) : DomainEvent();