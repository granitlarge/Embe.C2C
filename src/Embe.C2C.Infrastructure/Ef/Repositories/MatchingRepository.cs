using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class MatchingRepository(C2CContext context) : IMatchingRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<Matching> Set => new MyDbSet<Matching>(_context.Matchings);

    public async Task<Matching?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Matchings.SingleOrDefaultAsync(matching => matching.Id == id, cancellationToken);
    }

    public async Task<Matching?> GetMatchingByIdAsync
    (
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var matching = await _context
            .Matchings
            .AsSplitQuery()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.User1SearchProfile)
            .Include(m => m.User2SearchProfile)
            .Include(m => m.LastMessage)
            .Include(m => m.Messages!.OrderByDescending(mes => mes.CreatedAt).Take(50))
                .ThenInclude(m => m.ReplyToMessage)
            .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        return matching;
    }

    public async Task<Matching?> GetByMessageIdAsync
    (
        Guid messageId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Matchings.SingleOrDefaultAsync(m => m.Messages!.Any(msg => msg.Id == messageId), cancellationToken);
    }

    public async Task<List<Matching>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var matchings = await _context.Matchings
            .Where(m => m.UserId1 == userId || m.UserId2 == userId)
            .ToListAsync(cancellationToken);

        return matchings;
    }

    public async Task<List<Matching>> GetMatchingsAsync
    (
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var matchings = await _context.Matchings
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Include(m => m.User1SearchProfile)
            .Include(m => m.User2SearchProfile)
            .Include(m => m.LastMessage)
            .Where(m => m.UserId1 == userId || m.UserId2 == userId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return matchings;
    }

    public async Task<IsParticipantInMatchingFact> GetIsParticipantInMatchingFactAsync
    (
        Guid currentUserId,
        Guid matchingId,
        CancellationToken cancellationToken
    )
    {
        var isParticipantInMatching = await _context.Matchings
            .Where(m => m.Id == matchingId)
            .Where(m => m.UserId1 == currentUserId || m.UserId2 == currentUserId)
            .AnyAsync(cancellationToken);

        return new IsParticipantInMatchingFact(matchingId, isParticipantInMatching);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

}