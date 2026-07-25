using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IUserRepository : IGenericRepository<User, Guid>
{
    Task<bool> HasSearchProfilesAsync(Guid userId, CancellationToken cancellationToken);
    Task<AuthorizationFact[]> GetAuthorizationFactsAsync(Guid currentUserId, Guid targetUserId, CancellationToken cancellationToken);
    Task<User?> GetImageOwnerAsync(string imageName, CancellationToken cancellationToken);
}