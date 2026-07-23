namespace Embe.C2C.Domain.Aggregates.Candidates.Events;

public record JudgedEvent(Candidate Candidate) : DomainEvent();