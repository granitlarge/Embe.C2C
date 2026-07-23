using Embe.C2C.Domain.Aggregates.Accounts;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface IAccountRepository : IAggregateRepository<Account, Guid>
{
    Task<List<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}