using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface ISearchProfileRepository : IGenericRepository<SearchProfile, Guid>
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