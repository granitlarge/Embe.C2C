using System.Collections.Immutable;
using Embe.C2C.Domain;

namespace Embe.C2C.Application.Abstractions.Repos;

public interface ISaveChanges
{
    /// <summary>
    /// Persists changes made to retrieved entities.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IDbTransaction : IAsyncDisposable, IDisposable
{
    public Task CommitAsync(CancellationToken cancellationToken);
    public Task CreateSavePointAsync(string name, CancellationToken cancellationToken);
    public Task ReleaseSavePointAsync(string name, CancellationToken cancellationToken);
    public Task RollbackAsync(CancellationToken cancellationToken);
    public Task RollbackToSavePointAsync(string name, CancellationToken cancellationToken);
}

public interface IDbSet<T>
{
    void Add(T entity);
    void Remove(T entity);
}
public interface IBeginTransaction
{
    Task<IDbTransaction> BeginTransactionAsync(bool serializable, CancellationToken cancellationToken = default);
}

public interface IRepository : ISaveChanges, IBeginTransaction
{
    IImmutableList<DomainEvent> DomainEvents { get; }
}

public interface IGenericRepository<T_Aggregate, T_Aggregate_Id> : ISaveChanges
{
    IDbSet<T_Aggregate> Set { get; }
    Task<T_Aggregate?> GetByIdAsync(T_Aggregate_Id id, CancellationToken cancellationToken);
}