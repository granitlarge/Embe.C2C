using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Candidates.Events;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using ErrorOr;

namespace Embe.C2C.Domain.Services;

public class CandidateService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public CandidateService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

    public ErrorOr<Matching?> Judge
    (
        Candidate candidate,
        Candidate oppositeCandidate,
        bool isPositive
    )
    {
        candidate.Judge(isPositive);
        var isMatch = candidate.Judgement == true && oppositeCandidate.Judgement == true;
        if (!isMatch)
        {
            if (isPositive == true && oppositeCandidate.Judgement is null)
            {
                _domainEventStore.AddDomainEvent(new PositivelyJudgedDomainEvent(candidate));
            }
            return (Matching?)null;
        }

        var matching = Matching.Create
        (
            candidate.UserId,
            candidate.CandidateUserId,
            candidate.UserSearchProfileId,
            candidate.CandidateSearchProfileId
        );

        if (matching.IsError)
        {
            return matching.Errors;
        }

        _domainEventStore.AddDomainEvent(new MatchingCreatedDomainEvent(candidate.UserId, matching.Value));

        return matching.Value;
    }
}