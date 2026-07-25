using Embe.C2C.Domain.Aggregates.Candidates.Events;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Domain.Aggregates.Candidates;

public class Candidate : Aggregate
{
    private Candidate
    (
        Guid userId,
        Guid candidateUserId,
        Guid userSearchProfileId,
        Guid candidateSearchProfileId
    )
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        CandidateUserId = candidateUserId;
        UserSearchProfileId = userSearchProfileId;
        CandidateSearchProfileId = candidateSearchProfileId;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    private Candidate() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CandidateUserId { get; private set; }
    public Guid UserSearchProfileId { get; private set; }
    public Guid CandidateSearchProfileId { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public bool? Judgement { get; private set; }

    #region read-only navigation properties
    public User? User { get; private set; }
    public User? CandidateUser { get; private set; }
    public SearchProfile? UserSearchProfile { get; private set; }
    public SearchProfile? CandidateSearchProfile { get; private set; }
    #endregion

    internal void Judge(bool judgement)
    {
        if (Judgement == judgement)
            return;
        Judgement = judgement;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new JudgedEvent(this));
    }

    public static Candidate Create
    (
        Guid userId,
        Guid candidateUserId,
        Guid userSearchProfileId,
        Guid candidateSearchProfileId
    )
    {
        return new Candidate(userId, candidateUserId, userSearchProfileId, candidateSearchProfileId);
    }

    public void Remove()
    {
        AddDomainEvent(new RemovedEvent(this));
    }
}