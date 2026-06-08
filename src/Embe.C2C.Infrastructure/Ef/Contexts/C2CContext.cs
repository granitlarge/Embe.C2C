using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
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
) : IdentityDbContext<MyIdentityUser>(options), IC2CContext
{
    public DbSet<User> DomainUsers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Judgement> Judgements { get; set; }
    public DbSet<Matching> Matchings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

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
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }
}