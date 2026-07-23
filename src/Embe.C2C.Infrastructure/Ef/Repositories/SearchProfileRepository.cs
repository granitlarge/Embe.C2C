using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles.Facts;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class SearchProfileRepository(C2CContext context) : ISearchProfileRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<SearchProfile> Set => new MyDbSet<SearchProfile>(_context.SearchProfiles);

    public async Task<AuthorizationFact[]> GetAuthorizationFactsAsync
    (
        Guid currentUserId, 
        Guid searchProfileId, 
        CancellationToken cancellationToken
    )
    {
        var result = await _context.SearchProfiles
            .AsNoTracking()
            .Where(sp => sp.Id == searchProfileId)
            .Select(sp => new
            {
                sp.Id,
                IsOwnedByUser = sp.UserId == currentUserId,
                IsMatchedWithUser = sp.MatchingsUserId1!.Any(m => m.UserId2 == currentUserId) || sp.MatchingsUserId2!.Any(m => m.UserId1 == currentUserId),
                IsCandidateForUser = sp.User!.CandidateUsers!.Any(c => c.CandidateUserId == currentUserId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var facts = new AuthorizationFact[]
        {
            new IsOwnerFact(searchProfileId, result?.IsOwnedByUser ?? false),
            new IsMatchedFact(searchProfileId, result?.IsMatchedWithUser ?? false),
            new IsCandidateForUserFact(searchProfileId, result?.IsCandidateForUser ?? false)
        };

        return facts;
    }

    public Task<SearchProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.SearchProfiles.SingleOrDefaultAsync(sp => sp.Id == id, cancellationToken);
    }

    public Task<List<SearchProfile>> GetByUserIdAndHasMaximumDistanceFilter(Guid userId, CancellationToken cancellationToken)
    {
        return _context.SearchProfiles.Where(sp => sp.UserId == userId && sp.MaximumDistance != null).ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<SearchProfile>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        return _context.SearchProfiles
        .Where(sp => sp.UserId == userId)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
    }
}