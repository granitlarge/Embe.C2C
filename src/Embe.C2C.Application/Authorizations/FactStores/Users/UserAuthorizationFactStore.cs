using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Users.Facts;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations.FactStores.Users;

public class UserAuthorizationFactStore(IRepository repository, IAuthenticatedUserService authenticatedUserService) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly IRepository _repository = repository;

    public void SetCandidateUserFact(Guid userId, bool isCandidate)
    {
        var fact = new CandidateUserFact(userId, isCandidate);
        SetFact(fact);
    }

    public CandidateUserFact? GetCandidateUserFact(Guid userId)
    {
        return GetFact<CandidateUserFact>(userId);
    }

    public async ValueTask<BlockedByUserFact> GetBlockedByUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<BlockedByUserFact>(userId);
        if (fact is not null)
            return fact;

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<BlockedByUserFact>(userId) ?? throw new InvalidOperationException("BlockedByUserFact should have been loaded.");
    }

    public async ValueTask<BlockingUserFact> GetBlockingUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<BlockingUserFact>(userId);
        if (fact is not null)
            return fact;

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<BlockingUserFact>(userId) ?? throw new InvalidOperationException("BlockingUserFact should have been loaded.");
    }

    public async ValueTask<MatchedUserFact> GetMatchedUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<MatchedUserFact>(userId);
        if (fact is not null)
            return fact;

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<MatchedUserFact>(userId) ?? throw new InvalidOperationException("MatchedUserFact should have been loaded.");
    }

    public async ValueTask<SameUserFact> GetSameUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<SameUserFact>(userId) ?? SetFact(new SameUserFact(userId, userId == CurrentUserId));
        return fact;
    }

    private async Task LoadUserFactsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var facts = await _repository
            .DomainUsersQuery
            .Where(u => u.Id == CurrentUserId)
            .Select(u => new
            {
                IsBlocking = u.Blocked!.Any(bu => bu.BlockedUserId == userId),
                IsBlockedBy = u.BlockedBy!.Any(bu => bu.BlockerUserId == userId),
                IsMatched = u.Matchings1!.Any(m => m.UserId2 == userId) || u.Matchings2!.Any(m => m.UserId1 == userId),
            })
            .SingleOrDefaultAsync(cancellationToken);

        var blockedByFact = new BlockedByUserFact(userId, facts?.IsBlockedBy ?? false);
        var blockingFact = new BlockingUserFact(userId, facts?.IsBlocking ?? false);
        var matchedFact = new MatchedUserFact(userId, facts?.IsMatched ?? false);
        var sameFact = new SameUserFact(userId, userId == CurrentUserId);

        SetFact(blockedByFact);
        SetFact(blockingFact);
        SetFact(matchedFact);
        SetFact(sameFact);
    }

}