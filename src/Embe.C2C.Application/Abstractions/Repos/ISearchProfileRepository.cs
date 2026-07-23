using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.SearchProfiles;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface ISearchProfileRepository : IAggregateRepository<SearchProfile, Guid>
{
    public Task<List<SearchProfile>> GetByUserIdAndHasMaximumDistanceFilter(Guid userId, CancellationToken cancellationToken);
    public Task<AuthorizationFact[]> GetAuthorizationFactsAsync
    (
        Guid currentUserId,
        Guid searchProfileId,
        CancellationToken cancellationToken
    );
    public Task<List<SearchProfile>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
}