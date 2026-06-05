namespace Embe.C2C.Domain.Aggregates.Judgements.Events;

public record JudgementCreatedEvent(Judgement Judgement) : DomainEvent;