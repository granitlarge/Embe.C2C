using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Domain.Aggregates.Judgements;

namespace Embe.C2C.Application.Authorizations.FactStores.Judgements
{
    public class JudgementAuthorizationFactStore
    (
        IAuthenticatedUserService authenticatedUserService,
        UserAuthorizationFactStore userFactStore,
        JudgementAuthorizationFactGenerator judgementFactGenerator
    ) : AuthorizationFactStore(authenticatedUserService)
    {

        private readonly UserAuthorizationFactStore _userFactStore = userFactStore;
        private readonly JudgementAuthorizationFactGenerator _judgementFactGenerator = judgementFactGenerator;

        public async ValueTask<IsJudge> GetIsJudgeFactAsync(Judgement judgement)
        {
            var fact = GetFact<IsJudge>(judgement.Id);
            if (fact != null)
            {
                return fact;
            }

            await LoadAllFactsAsync(judgement.Id);
            return GetFact<IsJudge>(judgement.Id) ?? throw new InvalidOperationException($"Failed to load IsJudge fact for judgement with ID {judgement.Id}.");
        }

        public async ValueTask<IsJudgee> GetIsJudgeeFactAsync(Judgement judgement)
        {
            var fact = GetFact<IsJudgee>(judgement.Id);
            if (fact != null)
            {
                return fact;
            }

            await LoadAllFactsAsync(judgement.Id);
            return GetFact<IsJudgee>(judgement.Id) ?? throw new InvalidOperationException($"Failed to load IsJudgee fact for judgement with ID {judgement.Id}.");
        }

        public async ValueTask<IsPositivelyJudged> GetIsPositivelyJudgedFactAsync(Judgement judgement)
        {
            var fact = GetFact<IsPositivelyJudged>(judgement.Id);
            if (fact != null)
            {
                return fact;
            }
            await LoadAllFactsAsync(judgement.Id);
            return GetFact<IsPositivelyJudged>(judgement.Id) ?? throw new InvalidOperationException($"Failed to load IsPositivelyJudged fact for judgement with ID {judgement.Id}.");
        }

        private async Task LoadAllFactsAsync(Guid judgementId)
        {
            var facts = await _judgementFactGenerator.GetAuthorizationFactsAsync(judgementId);
            foreach (var fact in facts)
            {
                SetFact(fact);
            }
        }

    }

}