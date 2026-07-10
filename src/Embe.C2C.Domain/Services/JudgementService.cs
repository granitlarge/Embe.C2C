using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Domain.Services;

public class JudgementService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public JudgementService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

    public (Matching? Matching, Judgement Judgement) Judge
    (
        User judge,
        Candidate candidate,
        bool isPositive,
        Judgement? existingJudgement = null,
        Judgement? oppositeJudgement = null
    )
    {
        Judgement judgement;
        if (existingJudgement != null)
        {
            existingJudgement.Edit(isPositive);
            judgement = existingJudgement;
        }
        else
        {
            judgement = Judgement.Create(candidate.Id, isPositive);
        }

        var isMatch = judgement.IsPositive && oppositeJudgement?.IsPositive == true;
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

        return (match, judgement);
    }
}