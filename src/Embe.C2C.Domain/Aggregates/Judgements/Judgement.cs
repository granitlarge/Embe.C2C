using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Judgements.Events;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Aggregates.Judgements;

public class Judgement : Aggregate
{
    private Judgement
    (
        Guid candidateId,
        bool isPositive
    )
    {
        Id = Guid.CreateVersion7();
        CandidateId = candidateId;
        IsPositive = isPositive;
        CreatedAt = DateTimeOffset.UtcNow;
        EditedAt = CreatedAt;
        AddDomainEvent(new JudgementCreatedEvent(this));
    }

    private Judgement() { }

    public Guid Id { get; private set; }
    public Guid CandidateId { get; private set; }
    public bool IsPositive { get; private set; }
    public DateTimeOffset EditedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    #region read-only navigation properties
    public Candidate? Candidate { get; private set; }
    #endregion

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

    internal static Judgement Create(Guid candidateId, bool isPositive)
    {
        return new Judgement(candidateId, isPositive);
    }
}

public enum JudgementError
{
    SelfJudgement
}