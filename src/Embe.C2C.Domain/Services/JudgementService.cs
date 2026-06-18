using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Domain.Services;

public class JudgementService : DomainService
{
    public (Matching? Matching, Judgement Judgement) Judge
    (
        User judge,
        User judgee,
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
            judgement = Judgement.Judge(judge.Id, judgee.Id, isPositive);
        }

        var isMatch = judgement.IsPositive && oppositeJudgement?.IsPositive == true;
        var match = isMatch ? Matching.Create(judge.Id, judgee.Id) : null;
        if (match != null)
        {
            AddDomainEvent(new MatchingCreatedEvent(judge.Id, match));
        }

        return (match, judgement);
    }
}