using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;

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
    }

    private Candidate() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CandidateUserId { get; private set; }
    public Guid UserSearchProfileId { get; private set; }
    public Guid CandidateSearchProfileId { get; private set; }

    #region read-only navigation properties
    public User? User { get; private set; }
    public User? CandidateUser { get; private set; }
    public SearchProfile? UserSearchProfile { get; private set; }
    public SearchProfile? CandidateSearchProfile { get; private set; }
    public Judgement? Judgement { get; private set; }
    #endregion

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

    }
}