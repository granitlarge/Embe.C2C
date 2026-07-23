using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Users.Facts;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Entities;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Repositories;

public class UserRepository(C2CContext context) : IUserRepository
{
    private readonly C2CContext _context = context;

    public IDbSet<User> Set => new MyDbSet<User>(_context.DomainUsers);

    public async Task<AuthorizationFact[]> GetAuthorizationFactsAsync
    (
        Guid currentUserId,
        Guid targetUserId,
        CancellationToken cancellationToken
    )
    {
        var facts = await _context
            .DomainUsersQuery
            .Where(u => u.Id == currentUserId)
            .Select(u => new
            {
                IsBlocking = u.Blocked!.Any(bu => bu.BlockedUserId == targetUserId),
                IsBlockedBy = u.BlockedBy!.Any(bu => bu.BlockerUserId == targetUserId),
                IsMatched = u.Matchings1!.Any(m => m.UserId2 == targetUserId) || u.Matchings2!.Any(m => m.UserId1 == targetUserId),
                IsPositivelyJudged = u.CandidateCandidates!.Any(c => c.UserId == targetUserId && c.Judgement == true)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var blockedByFact = new BlockedByUserFact(targetUserId, facts?.IsBlockedBy ?? false);
        var blockingFact = new BlockingUserFact(targetUserId, facts?.IsBlocking ?? false);
        var matchedFact = new MatchedUserFact(targetUserId, facts?.IsMatched ?? false);
        var sameFact = new SameUserFact(targetUserId, targetUserId == currentUserId);
        var positivelyJudgedFact = new IsPositivelyJudgedByUser(targetUserId, facts?.IsPositivelyJudged ?? false);

        return
        [
            blockedByFact,
            blockingFact,
            matchedFact,
            sameFact,
            positivelyJudgedFact
        ];
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.DomainUsers.SingleOrDefaultAsync(du => du.Id == id, cancellationToken);
    }

    public async Task<User?> GetImageOwnerAsync(Guid imageId, CancellationToken cancellationToken)
    {
        var user = await _context.DomainUsers.Where(du => EF.Property<List<Image>>(du, "_images").Any(image => image.Id == imageId)).SingleOrDefaultAsync(cancellationToken);
        return user;
    }

    public async Task<bool> HasSearchProfilesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.DomainUsers.Where(du => du.Id == userId).AnyAsync(du => du.SearchProfiles!.Any(), cancellationToken: cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}