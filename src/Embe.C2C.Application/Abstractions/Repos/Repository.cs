using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Blockings;
using Embe.C2C.Domain.Aggregates.Candidates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Application.Abstractions.Repos
{
    public interface IDbSet<T>
    {
        void Add(T entity);
        void Remove(T entity);
    }

    public interface ISparseRepository
    {
        public IDbSet<Account> Accounts { get; }
        public IDbSet<Matching> Matchings { get; }
        public IDbSet<Notification> Notifications { get; }
        public IDbSet<Message> Messages { get; }
        public IDbSet<Blocking> Blockings { get; }
        public IDbSet<SearchProfile> SearchProfiles { get; }
        public IDbSet<Candidate> Candidates { get; }

        public IQueryable<Account> AccountsQuery { get; }
        public IQueryable<Matching> MatchingsQuery { get; }
        public IQueryable<Notification> NotificationsQuery { get; }
        public IQueryable<Message> MessagesQuery { get; }
        public IQueryable<Blocking> BlockingsQuery { get; }
        public IQueryable<SearchProfile> SearchProfilesQuery { get; }
        public IQueryable<IAdminArea> AdminAreasQuery { get; }
        public IQueryable<Candidate> CandidatesQuery { get; }

        public Task<bool> GenerateCandidatesForUserIdAsync
        (
            Guid userId,
            CancellationToken cancellationToken = default
        );

        public Task<List<IAdminArea>> SearchAdminAreasAsync
        (
            string? parentId,
            double? longitude,
            double? latitude,
            int page,
            int size,
            CancellationToken cancellationToken = default
        );
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IRepository : ISparseRepository
    {
        public IImmutableList<DomainEvent> DomainEvents { get; }
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        public Task<List<IAdminArea>> ReverseGeocodeAsync(double longitude, double latitude);
    }

    public class SparseRepository : ISparseRepository
    {
        private readonly IRepository _context;

        public SparseRepository(IRepository context)
        {
            _context = context;
        }

        public IDbSet<Account> Accounts => _context.Accounts;
        public IDbSet<Matching> Matchings => _context.Matchings;
        public IDbSet<Notification> Notifications => _context.Notifications;
        public IDbSet<Message> Messages => _context.Messages;
        public IDbSet<Blocking> Blockings => _context.Blockings;
        public IDbSet<SearchProfile> SearchProfiles => _context.SearchProfiles;
        public IQueryable<Account> AccountsQuery => _context.AccountsQuery;
        public IQueryable<Matching> MatchingsQuery => _context.MatchingsQuery;
        public IQueryable<Notification> NotificationsQuery => _context.NotificationsQuery;
        public IQueryable<SearchProfile> SearchProfilesQuery => _context.SearchProfilesQuery;
        public IQueryable<Message> MessagesQuery => _context.MessagesQuery;
        public IQueryable<Blocking> BlockingsQuery => _context.BlockingsQuery;
        public IQueryable<Candidate> CandidatesQuery => _context.CandidatesQuery;

        public IQueryable<IAdminArea> AdminAreasQuery => _context.AdminAreasQuery;

        public IDbSet<Candidate> Candidates => _context.Candidates;

        public async Task<bool> GenerateCandidatesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.GenerateCandidatesForUserIdAsync(userId, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<IAdminArea>> SearchAdminAreasAsync
        (
            string? parentId,
            double? longitude,
            double? latitude,
            int page,
            int size,
            CancellationToken cancellationToken = default
        )
        {
            return await _context.SearchAdminAreasAsync(parentId, longitude, latitude, page, size, cancellationToken);
        }
    }
}