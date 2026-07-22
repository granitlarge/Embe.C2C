using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Users.Facts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactGenerators;

public class UserFactGenerator
(
    IRepository repo,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactGenerator(authenticatedUserService)
{
    private readonly IRepository _repo = repo;

    public async Task<BlockedByUserFact> GetBlockedByUserFactAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var facts = await GetAllFactsAsync(userId, cancellationToken);
        return facts.OfType<BlockedByUserFact>().Single();
    }

    public async Task<BlockingUserFact> GetBlockingUserFactAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var facts = await GetAllFactsAsync(userId, cancellationToken);
        return facts.OfType<BlockingUserFact>().Single();
    }

    public async Task<MatchedUserFact> GetMatchedUserFactAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var facts = await GetAllFactsAsync(userId, cancellationToken);
        return facts.OfType<MatchedUserFact>().Single();
    }

    public async Task<AuthorizationFact[]> GetAllFactsAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _repo.DomainUsersQuery
            .Include(u => u.Blocked!.Where(bu => bu.BlockedUserId == userId))
            .Include(u => u.BlockedBy!.Where(bu => bu.BlockerUserId == userId))
            .Include(u => u.Matchings1!.Where(m => m.UserId2 == userId))
            .Include(u => u.Matchings2!.Where(m => m.UserId1 == userId))
            .Include(u => u.CandidateCandidates!.Where(c => c.UserId == userId))
                .ThenInclude(cc => cc.Judgement)
            .SingleOrDefaultAsync(u => u.Id == CurrentUserId, cancellationToken);

        var facts = user != null ? new
        {
            IsBlocking = user.Blocked!.Any(bu => bu.BlockedUserId == userId),
            IsBlockedBy = user.BlockedBy!.Any(bu => bu.BlockerUserId == userId),
            IsMatched = user.Matchings1!.Any(m => m.UserId2 == userId) || user.Matchings2!.Any(m => m.UserId1 == userId),
            IsPositivelyJudged = user.CandidateCandidates!.Any(c => c.UserId == userId && c.Judgement!.IsPositive)
        } : null;

        var blockedByFact = new BlockedByUserFact(userId, facts?.IsBlockedBy ?? false);
        var blockingFact = new BlockingUserFact(userId, facts?.IsBlocking ?? false);
        var matchedFact = new MatchedUserFact(userId, facts?.IsMatched ?? false);
        var sameFact = new SameUserFact(userId, userId == CurrentUserId);
        var positivelyJudgedFact = new IsPositivelyJudged(userId, facts?.IsPositivelyJudged ?? false);

        return
        [
            blockedByFact,
            blockingFact,
            matchedFact,
            sameFact,
            positivelyJudgedFact
        ];
    }
}