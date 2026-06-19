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
        var facts = await _repo
            .DomainUsersQuery
            .Where(u => u.Id == CurrentUserId)
            .Select(u => new
            {
                IsBlocking = u.Blocked!.Any(bu => bu.BlockedUserId == userId),
                IsBlockedBy = u.BlockedBy!.Any(bu => bu.BlockerUserId == userId),
                IsMatched = u.Matchings1!.Any(m => m.UserId2 == userId) || u.Matchings2!.Any(m => m.UserId1 == userId),
                IsPositivelyJudged = u.JudgementsReceived!.Any(j => j.JudgeUserId == userId && j.IsPositive)
            })
            .SingleOrDefaultAsync(cancellationToken);

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