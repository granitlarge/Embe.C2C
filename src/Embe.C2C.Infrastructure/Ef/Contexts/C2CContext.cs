using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Infrastructure.Ef.Entities;
using Embe.C2C.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Contexts;

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

public class C2CContext
(
    DbContextOptions<C2CContext> options
) : IdentityDbContext<MyIdentityUser>(options), IRepository
{
    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Judgement> Judgements { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<Message> Messages { get; set; }


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

    public IQueryable<User> DomainUsersQuery
    {
        get
        {
            return DomainUsers;
        }
    }

    public IQueryable<Account> AccountsQuery
    {
        get
        {
            return Accounts;
        }
    }

    public IQueryable<Judgement> JudgementsQuery
    {
        get
        {
            return Judgements;
        }
    }

    public IQueryable<Matching> MatchingsQuery
    {
        get
        {
            return Matchings.Include(m => m.Conversation);
        }
    }

    public IQueryable<Notification> NotificationsQuery
    {
        get
        {
            return Notifications;
        }
    }

    IDbSet<User> ISparseRepository.DomainUsers
    {
        get
        {
            return new MyDbSet<User>(DomainUsers);
        }
    }

    IDbSet<Account> ISparseRepository.Accounts
    {
        get
        {
            return new MyDbSet<Account>(Accounts);
        }
    }

    IDbSet<Judgement> ISparseRepository.Judgements
    {
        get
        {
            return new MyDbSet<Judgement>(Judgements);
        }
    }

    IDbSet<Matching> ISparseRepository.Matchings
    {
        get
        {
            return new MyDbSet<Matching>(Matchings);
        }
    }

    IDbSet<Notification> ISparseRepository.Notifications
    {
        get
        {
            return new MyDbSet<Notification>(Notifications);
        }
    }

    public IQueryable<Message> MessagesQuery => Messages;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(System.Data.IsolationLevel.Snapshot, cancellationToken);
    }

    public async Task<IQueryable<User>> GetCandidatesForUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}