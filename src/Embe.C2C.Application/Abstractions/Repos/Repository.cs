using System.Collections.Immutable;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Judgements;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Application.Abstractions.Repos
{
    public interface IDbSet<T>
    {
        void Add(T entity);
        void Remove(T entity);
        ValueTask<T?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken = default);
    }

    public interface ISparseRepository
    {
        public IDbSet<User> DomainUsers { get; }
        public IDbSet<Account> Accounts { get; }
        public IDbSet<Judgement> Judgements { get; }
        public IDbSet<Matching> Matchings { get; }
        public IDbSet<Notification> Notifications { get; }

        public IQueryable<User> DomainUsersQuery { get; }
        public IQueryable<Account> AccountsQuery { get; }
        public IQueryable<Judgement> JudgementsQuery { get; }
        public IQueryable<Matching> MatchingsQuery { get; }
        public IQueryable<Notification> NotificationsQuery { get; }

        public Task<IQueryable<User>> GetCandidatesForUserIdAsync(Guid userId);
    }

    public interface IRepository : ISparseRepository
    {
        public IImmutableList<DomainEvent> DomainEvents { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    public class SparseRepository : ISparseRepository
    {
        private readonly IRepository _context;

        public SparseRepository(IRepository context)
        {
            _context = context;
        }

        public IDbSet<User> DomainUsers => _context.DomainUsers;
        public IDbSet<Account> Accounts => _context.Accounts;
        public IDbSet<Judgement> Judgements => _context.Judgements;
        public IDbSet<Matching> Matchings => _context.Matchings;
        public IDbSet<Notification> Notifications => _context.Notifications;

        public IQueryable<User> DomainUsersQuery => _context.DomainUsersQuery;
        public IQueryable<Account> AccountsQuery => _context.AccountsQuery;
        public IQueryable<Judgement> JudgementsQuery => _context.JudgementsQuery;
        public IQueryable<Matching> MatchingsQuery => _context.MatchingsQuery;
        public IQueryable<Notification> NotificationsQuery => _context.NotificationsQuery;

        public async Task<IQueryable<User>> GetCandidatesForUserIdAsync(Guid userId)
        {
            return await _context.GetCandidatesForUserIdAsync(userId);
        }
    }
}