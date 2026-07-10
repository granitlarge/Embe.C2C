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
        var result = await _repository
            .JudgementsQuery
            .AsNoTracking()
            .Where(j => j.Id == judgementId)
            .Select(j => new
            {
                IsJudge = j.Candidate!.UserId == CurrentUserId,
                IsJudgee = j.Candidate!.CandidateUserId == CurrentUserId,
                IsPositivelyJudged = j.Candidate!.CandidateUserId == CurrentUserId && j.IsPositive
            })
            .FirstOrDefaultAsync();

        var facts = new List<AuthorizationFact>
        {
            new IsJudge(judgementId, result?.IsJudge ?? false),
            new IsJudgee(judgementId, result?.IsJudgee ?? false),
            new IsPositivelyJudged(judgementId, result?.IsPositivelyJudged ?? false)
        };
        return facts;
    }
}