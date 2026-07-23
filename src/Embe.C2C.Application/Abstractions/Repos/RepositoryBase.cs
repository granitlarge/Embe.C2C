using System.Collections.Immutable;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface ISaveChanges
{
    /// <summary>
    /// Persists changes made to retrieved entities.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface INewRepository : ISaveChanges
{
    IImmutableList<DomainEvent> DomainEvents { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

public interface IAggregateRepository<T_Aggregate, T_Aggregate_Id> : ISaveChanges
{
    IDbSet<T_Aggregate> Set { get; }
    Task<T_Aggregate?> GetByIdAsync(T_Aggregate_Id id, CancellationToken cancellationToken);
}