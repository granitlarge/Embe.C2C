namespace Embe.C2C.Domain.Aggregates.Judgements.Events;

public record JudgementEditedEvent(Judgement Judgement) : DomainEvent;