using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Entities;
using Embe.C2C.Domain;
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
        public IQueryable<IAdminArea> AdminAreasQuery { get; }

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

        public IQueryable<IAdminArea> AdminAreasQuery => _context.AdminAreasQuery;

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