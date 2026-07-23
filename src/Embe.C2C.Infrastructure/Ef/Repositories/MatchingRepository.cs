using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class MatchingRepository(C2CContext context) : IMatchingRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<Matching> Set => new MyDbSet<Matching>(_context.Matchings);

    public async Task<Matching?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.MatchingsQuery.SingleOrDefaultAsync(matching => matching.Id == id, cancellationToken);
    }

    public async Task<Matching?> GetByIdAsync
    (
        Guid id,
        bool includeUser1,
        bool includeUser2,
        bool includeUser1SearchProfile,
        bool includeUser2SearchProfile,
        bool includeLastMessage,
        bool includeMessages,
        bool includeMessagesReplyToMessage,
        int numberOfMessagesToInclude,
        CancellationToken cancellationToken
    )
    {
        var query = _context.MatchingsQuery;

        if (includeUser1)
        {
            query = query.Include(m => m.User1);
        }

        if (includeUser2)
        {
            query = query.Include(m => m.User2);
        }

        if (includeUser1SearchProfile)
        {
            query = query.Include(m => m.User1SearchProfile);
        }

        if (includeUser2SearchProfile)
        {
            query = query.Include(m => m.User2SearchProfile);
        }

        if (includeLastMessage)
        {
            query = query.Include(m => m.LastMessage);
        }

        if (includeMessages)
        {
            if (includeMessagesReplyToMessage)
            {
                query = query
                    .Include(m => m.Messages!.OrderByDescending(mes => mes.CreatedAt).Take(numberOfMessagesToInclude))
                    .ThenInclude(m => m.ReplyToMessage);
            }
            else
            {
                query = query.Include(m => m.Messages!.OrderByDescending(mes => mes.CreatedAt).Take(numberOfMessagesToInclude));
            }
        }

        return await query.AsSplitQuery().SingleOrDefaultAsync(matching => matching.Id == id, cancellationToken);
    }

    public async Task<Matching?> GetByMessageIdAsync
    (
        Guid messageId,
        CancellationToken cancellationToken
    )
    {
        return await _context.MatchingsQuery.SingleOrDefaultAsync(m => m.Messages!.Any(msg => msg.Id == messageId), cancellationToken);
    }

    public Task<List<Matching>> GetByUserIdAsync
    (
        Guid userId,
        bool includeUser1,
        bool includeUser2,
        bool includeUser1SearchProfile,
        bool includeUser2SearchProfile,
        bool includeLastMessage,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = _context.MatchingsQuery;
        if (includeUser1)
        {
            query = query.Include(m => m.User1);
        }

        if (includeUser2)
        {
            query = query.Include(m => m.User2);
        }

        if (includeUser1SearchProfile)
        {
            query = query.Include(m => m.User1SearchProfile);
        }

        if (includeUser2SearchProfile)
        {
            query = query.Include(m => m.User2SearchProfile);
        }

        if (includeLastMessage)
        {
            query = query.Include(m => m.LastMessage);
        }

        var result = query
            .AsSplitQuery()
            .Where(m => m.UserId1 == userId || m.UserId2 == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<IsParticipantInMatchingFact> GetIsParticipantInMatchingFactAsync
    (
        Guid currentUserId,
        Guid matchingId,
        CancellationToken cancellationToken
    )
    {
        var isParticipantInMatching = await _context.MatchingsQuery
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