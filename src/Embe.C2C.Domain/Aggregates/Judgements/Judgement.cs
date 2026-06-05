using Embe.C2C.Domain.Aggregates.Judgements.Events;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Aggregates.Judgements;

public class Judgement : Aggregate
{
    private Judgement
    (
        Guid judgeUserId,
        Guid judgeeUserId,
        bool isPositive
    )
    {
        if (judgeUserId == judgeeUserId)
        {
            throw new DomainException("A user cannot judge themselves.");
        }

        Id = Guid.CreateVersion7();
        JudgeUserId = judgeUserId;
        JudgeeUserId = judgeeUserId;
        IsPositive = isPositive;
        CreatedAt = DateTimeOffset.UtcNow;
        EditedAt = CreatedAt;
        AddDomainEvent(new JudgementCreatedEvent(this));
    }

    public Guid Id { get; }
    public Guid JudgeUserId { get; }
    public Guid JudgeeUserId { get; }
    public bool IsPositive { get; private set; }
    public DateTimeOffset EditedAt { get; private set;}
    public DateTimeOffset CreatedAt { get; }

    public void Edit(bool isPositive)
    {
        if (IsPositive == isPositive)
        {
            return;
        }

        IsPositive = isPositive;
        EditedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new JudgementEditedEvent(this));
    }

    public void Remove()
    {

    }

    internal static Judgement Judge(Guid judgeUserId, Guid judgeeUserId, bool isPositive)
    {
        return new Judgement(judgeUserId, judgeeUserId, isPositive);
    }
}