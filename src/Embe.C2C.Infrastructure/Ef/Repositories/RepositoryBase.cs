using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class Repository(C2CContext context) : INewRepository
{
    private readonly C2CContext _context = context;

    public IImmutableList<DomainEvent> DomainEvents => _context.DomainEvents;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.BeginTransactionAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}