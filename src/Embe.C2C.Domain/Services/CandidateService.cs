using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Domain.Services;

public class CandidateService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public CandidateService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

    public Matching? Judge
    (
        User judge,
        Candidate candidate,
        Candidate oppositeCandidate,
        bool isPositive
    )
    {
        candidate.Judge(isPositive);
        var isMatch = candidate.Judgement == true && oppositeCandidate.Judgement == true;
        var match = isMatch ? Matching.Create
        (
            judge.Id,
            candidate.CandidateUserId,
            candidate.UserSearchProfileId,
            candidate.CandidateSearchProfileId
        ) : null;

        if (match != null)
        {
            _domainEventStore.AddDomainEvent(new MatchingCreatedEvent(judge.Id, match));
        }

        return match;
    }
}