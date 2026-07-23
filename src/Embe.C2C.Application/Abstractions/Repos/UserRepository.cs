using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IUserRepository : IAggregateRepository<User, Guid>
{
    Task<bool> HasSearchProfilesAsync(Guid userId, CancellationToken cancellationToken);
    Task<AuthorizationFact[]> GetAuthorizationFactsAsync(Guid currentUserId, Guid targetUserId, CancellationToken cancellationToken);
    Task<User?> GetImageOwnerAsync(Guid imageId, CancellationToken cancellationToken);
}