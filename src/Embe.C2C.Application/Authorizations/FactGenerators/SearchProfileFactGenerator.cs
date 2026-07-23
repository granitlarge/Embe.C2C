using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class SearchProfileFactGenerator
(
    ISearchProfileRepository searchProfileRepository,
    IRepository repository,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IRepository _repository = repository;
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;

    public async Task<List<AuthorizationFact>> GetAuthorizationFactsAsync(Guid searchProfileId, CancellationToken cancellationToken = default)
    {
        return [.. await _searchProfileRepository.GetAuthorizationFactsAsync(CurrentUserId ?? Guid.Empty, searchProfileId, cancellationToken)];
    }
}