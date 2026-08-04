using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Entities;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Contexts;

public class C2CContext
(
    DbContextOptions<C2CContext> options
) : IdentityDbContext<MyIdentityUser>(options), IRepository
{
    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Domain.Aggregates.Accounts.Account> Accounts { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Blocking> Blockings { get; set; }
    public DbSet<SearchProfile> SearchProfiles { get; set; }
    public DbSet<AdminArea> AdminAreas { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<SearchProfileEmbedding> SearchProfileEmbeddings { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; internal set; }

    public IImmutableList<DomainEvent> DomainEvents
    {
        get
        {
            return ChangeTracker.Entries()
                .Select(e => e.Entity)
                .OfType<DomainEventCollector>()
                .SelectMany(c => c.DomainEvents)
                .ToImmutableList();
        }
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public async Task<IDbTransaction> BeginTransactionAsync(bool serializable, CancellationToken cancellationToken)
    {
        var transaction = await Database.BeginTransactionAsync(serializable ? System.Data.IsolationLevel.Serializable : System.Data.IsolationLevel.Snapshot, cancellationToken);
        return new MyDbTransaction(transaction);
    }
}

public class MyDbSet<T>(DbSet<T> dbSet) : IDbSet<T> where T : class
{
    private readonly DbSet<T> _dbSet = dbSet;

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public ValueTask<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync(keyValues, cancellationToken);
    }
}

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