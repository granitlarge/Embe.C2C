using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class CandidateAuthorizationFactGenerator
(
    ICandidateRepository candidateRepository,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly ICandidateRepository _candidateRepository = candidateRepository;

    public async Task<List<AuthorizationFact>> GetAuthorizationFactsAsync(Guid candidateId, CancellationToken cancellationToken)
    {
        var facts = await _candidateRepository.GetAuthorizationFactsAsync(CurrentUserId ?? Guid.Empty, candidateId, cancellationToken);
        return facts;
    }
}