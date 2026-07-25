using Embe.C2C.Domain.Aggregates.Accounts;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IAccountRepository : IGenericRepository<Domain.Aggregates.Accounts.Account, Guid>
{
    Task<List<Domain.Aggregates.Accounts.Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}