using Embe.C2C.Application.Abstractions.Repos;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class MyDbTransaction : IDbTransaction
{
    private readonly IDbContextTransaction _dbContextTransaction;

    public MyDbTransaction(IDbContextTransaction dbContextTransaction)
    {
        _dbContextTransaction = dbContextTransaction;
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return _dbContextTransaction.CommitAsync(cancellationToken);
    }

    public Task CreateSavePointAsync(string name, CancellationToken cancellationToken)
    {
        return _dbContextTransaction.CreateSavepointAsync(name, cancellationToken);
    }

    public void Dispose()
    {
        _dbContextTransaction.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _dbContextTransaction.DisposeAsync();
    }

    public Task ReleaseSavePointAsync(string name, CancellationToken cancellationToken)
    {
        return _dbContextTransaction.ReleaseSavepointAsync(name, cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        return _dbContextTransaction.RollbackAsync(cancellationToken);
    }

    public Task RollbackToSavePointAsync(string name, CancellationToken cancellationToken)
    {
        return _dbContextTransaction.RollbackToSavepointAsync(name, cancellationToken);
    }
}