using System.Collections.Immutable;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Application.Abstractions.Repos
{
    public interface ISparseC2CContext
    {
        public DbSet<User> DomainUsers { get; }
        public DbSet<Account> Accounts { get; }
        public DbSet<Judgement> Judgements { get; }
        public DbSet<Matching> Matchings { get; }
        public DbSet<Notification> Notifications { get; }
    }

    public interface IC2CContext : ISparseC2CContext
    {
        public IImmutableList<DomainEvent> DomainEvents { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    public class SparseC2CContext : ISparseC2CContext
    {
        private readonly IC2CContext _context;

        public SparseC2CContext(IC2CContext context)
        {
            _context = context;
        }

        public DbSet<User> DomainUsers => _context.DomainUsers;
        public DbSet<Account> Accounts => _context.Accounts;
        public DbSet<Judgement> Judgements => _context.Judgements;
        public DbSet<Matching> Matchings => _context.Matchings;
        public DbSet<Notification> Notifications => _context.Notifications;
    }
}