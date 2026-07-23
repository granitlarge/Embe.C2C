namespace Embe.C2C.Domain.Aggregates.Candidates.Events;

public record RemovedEvent(Candidate Candidate) : DomainEvent;