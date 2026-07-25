using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class AccountRepository(C2CContext context) : IAccountRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<Domain.Aggregates.Accounts.Account> Set => new MyDbSet<Domain.Aggregates.Accounts.Account>(_context.Accounts);

    public async Task<Domain.Aggregates.Accounts.Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Accounts.SingleOrDefaultAsync(account => account.Id == id, cancellationToken);
    }

    public async Task<List<Domain.Aggregates.Accounts.Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Accounts.Where(account => account.UserId == userId).ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}