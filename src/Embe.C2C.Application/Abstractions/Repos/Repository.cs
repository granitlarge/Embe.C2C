using System.Collections.Immutable;
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
        public Task<bool> GenerateCandidatesForUserIdAsync
        (
            Guid userId,
            CancellationToken cancellationToken = default
        );

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IRepository : ISparseRepository
    {
        public IImmutableList<DomainEvent> DomainEvents { get; }
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    public class SparseRepository : ISparseRepository
    {
        private readonly IRepository _context;

        public SparseRepository(IRepository context)
        {
            _context = context;
        }

        public async Task<bool> GenerateCandidatesForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.GenerateCandidatesForUserIdAsync(userId, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }

}