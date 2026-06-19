using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Domain.Aggregates.Judgements;

namespace Embe.C2C.Application.Authorizations.FactStores.Judgements
{

    public class JudgementAuthorizationFactStore
    (
        IAuthenticatedUserService authenticatedUserService,
        UserAuthorizationFactStore userFactStore
    ) : AuthorizationFactStore(authenticatedUserService)
    {

        private readonly UserAuthorizationFactStore _userFactStore = userFactStore;

        public IsJudge GetIsJudgeFact(Judgement judgement)
        {
            var fact = GetFact<IsJudge>(judgement.Id);
            if (fact != null)
            {
                return fact;
            }

            return SetFact(new IsJudge
            (
                judgement.Id,
                judgement.JudgeUserId == CurrentUserId
            ));
        }

        public IsJudgee GetIsJudgeeFact(Judgement judgement)
        {
            var fact = GetFact<IsJudgee>(judgement.Id);
            if (fact != null)
            {
                return fact;
            }

            return SetFact(new IsJudgee
            (
                judgement.Id,
                judgement.JudgeeUserId == CurrentUserId
            ));
        }

        public IsPositivelyJudged GetIsPositivelyJudgedFact(Judgement judgement)
        {
            var fact = GetFact<IsPositivelyJudged>(judgement.Id);
            if (fact != null)
            {
                return fact;
            }

            fact = new IsPositivelyJudged
            (
                judgement.Id,
                judgement.JudgeeUserId == CurrentUserId && judgement.IsPositive
            );

            if (judgement.JudgeeUserId == CurrentUserId)
                _userFactStore.SetIsPositivelyJudgedByUserFact(judgement.JudgeUserId, fact.Value);

            return SetFact(fact);
        }

    }

}