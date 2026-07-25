using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IBlockingRepository : IGenericRepository<Blocking, Guid>
{
    public Task<Blocking?> GetByUserIdsAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken);
}