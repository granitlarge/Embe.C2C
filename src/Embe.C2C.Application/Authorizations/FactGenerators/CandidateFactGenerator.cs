using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Candidates.Facts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class CandidateAuthorizationFactGenerator
(
    IRepository repository,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IRepository _repository = repository;

    public async Task<List<AuthorizationFact>> GetAuthorizationFactsAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        var facts = await _repository.CandidatesQuery
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                IsOwner = c.UserId == CurrentUserId,
                IsCandidate = c.CandidateUserId == CurrentUserId
            })
            .SingleOrDefaultAsync(c => c.Id == candidateId, cancellationToken);

        return
        [
            new IsOwner(candidateId, facts?.IsOwner ?? false),
            new IsCandidate(candidateId, facts?.IsCandidate ?? false)
        ];
    }
}