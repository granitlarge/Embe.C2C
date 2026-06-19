using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactGenerators;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Users.Facts;

namespace Embe.C2C.Application.Authorizations.FactStores.Users;

public class UserAuthorizationFactStore
(
    UserFactGenerator factGenerator,
    IAuthenticatedUserService authenticatedUserService
) : AuthorizationFactStore(authenticatedUserService)
{
    private readonly UserFactGenerator _factGenerator = factGenerator;

    public void SetCandidateUserFact(Guid userId, bool isCandidate)
    {
        var fact = new CandidateUserFact(userId, isCandidate);
        SetFact(fact);
    }

    public void SetIsPositivelyJudgedByUserFact(Guid userId, bool isPositivelyJudgedByUser)
    {
        var fact = new IsPositivelyJudgedByUser(userId, isPositivelyJudgedByUser);
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

        if (userId == CurrentUserId)
            return new BlockedByUserFact(userId, false);

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<BlockedByUserFact>(userId) ?? throw new InvalidOperationException("BlockedByUserFact should have been loaded.");
    }

    public async ValueTask<BlockingUserFact> GetBlockingUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<BlockingUserFact>(userId);
        if (fact is not null)
            return fact;

        if (userId == CurrentUserId)
            return new BlockingUserFact(userId, false);

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<BlockingUserFact>(userId) ?? throw new InvalidOperationException("BlockingUserFact should have been loaded.");
    }

    public async ValueTask<MatchedUserFact> GetMatchedUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<MatchedUserFact>(userId);
        if (fact is not null)
            return fact;

        if (userId == CurrentUserId)
            return new MatchedUserFact(userId, false);

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<MatchedUserFact>(userId) ?? throw new InvalidOperationException("MatchedUserFact should have been loaded.");
    }

    public SameUserFact GetSameUserFact(Guid userId)
    {
        var fact = GetFact<SameUserFact>(userId) ?? SetFact(new SameUserFact(userId, userId == CurrentUserId));
        return fact;
    }

    public async ValueTask<IsPositivelyJudged> GetIsPositivelyJudgedByUserFactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var fact = GetFact<IsPositivelyJudged>(userId);
        if (fact != null)
        {
            return fact;
        }

        await LoadUserFactsAsync(userId, cancellationToken);
        return GetFact<IsPositivelyJudged>(userId) ?? throw new InvalidOperationException("IsPositivelyJudgedJudgee fact should have been loaded.");
    }

    private async Task LoadUserFactsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var facts = await _factGenerator.GetAllFactsAsync(userId, cancellationToken);
        foreach (var fact in facts)
        {
            SetFact(fact);
        }
    }

}