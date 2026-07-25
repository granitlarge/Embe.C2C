using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;
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
        User judge,
        Candidate candidate,
        Candidate oppositeCandidate,
        bool isPositive
    )
    {
        candidate.Judge(isPositive);
        var isMatch = candidate.Judgement == true && oppositeCandidate.Judgement == true;
        if (!isMatch)
            return (Matching?)null;

        var matching = Matching.Create
        (
            judge.Id,
            oppositeCandidate.CandidateUserId,
            oppositeCandidate.UserSearchProfileId,
            oppositeCandidate.CandidateSearchProfileId
        );

        if (matching.IsError)
        {
            return matching.Errors;
        }

        _domainEventStore.AddDomainEvent(new MatchingCreatedEvent(judge.Id, matching.Value));

        return matching.Value;
    }
}