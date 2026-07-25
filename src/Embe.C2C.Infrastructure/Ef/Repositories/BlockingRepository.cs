using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class BlockingRepository(C2CContext context) : IBlockingRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<Blocking> Set => new MyDbSet<Blocking>(_context.Blockings);

    public Task<Blocking?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Blockings.SingleOrDefaultAsync(blocking => blocking.Id == id, cancellationToken);
    }

    public Task<Blocking?> GetByUserIdsAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken)
    {
        return _context.Blockings.SingleOrDefaultAsync(blocking => blocking.BlockerUserId == blockedUserId && blocking.BlockedUserId == blockedUserId, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}