using Embe.C2C.Domain.Aggregates.Blockings;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IBlockingRepository : IAggregateRepository<Blocking, Guid>
{
    public Task<Blocking?> GetByUserIdsAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken);
}