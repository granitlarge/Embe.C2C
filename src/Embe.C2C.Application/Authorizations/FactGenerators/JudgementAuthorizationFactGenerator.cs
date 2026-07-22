using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class JudgementAuthorizationFactGenerator
(
    IRepository repository,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IRepository _repository = repository;

    public async Task<List<AuthorizationFact>> GetAuthorizationFactsAsync(Guid judgementId)
    {
        var judgement = await _repository
            .JudgementsQuery
            .Include(j => j.Candidate)
            .SingleOrDefaultAsync(j => j.Id == judgementId);

        var result = judgement != null ? new
        {
            IsJudge = judgement.Candidate!.UserId == CurrentUserId,
            IsJudgee = judgement.Candidate!.CandidateUserId == CurrentUserId,
            IsPositivelyJudged = judgement.Candidate!.CandidateUserId == CurrentUserId && judgement.IsPositive
        } : null;

        var facts = new List<AuthorizationFact>
        {
            new IsJudge(judgementId, result?.IsJudge ?? false),
            new IsJudgee(judgementId, result?.IsJudgee ?? false),
            new IsPositivelyJudged(judgementId, result?.IsPositivelyJudged ?? false)
        };
        return facts;
    }
}