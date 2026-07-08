using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.FactStores.Judgements.Facts;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Application.Authorizations.FactStores.Users.Facts;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Authorizations;

public class UserAuthorizationService
{
    private readonly UserAuthorizationFactStore _facts;

    private readonly IFileUrlGenerator _fileUrlGenerator;

    public UserAuthorizationService
    (
        UserAuthorizationFactStore facts,
        IFileService fileService
    )
    {
        _facts = facts;
        _fileUrlGenerator = new FileUrlGenerator(fileService, TimeSpan.FromSeconds(15));
    }
/*
    public async Task<ReadDto<UserDto, UserPermission>?> ToDtoAsync
    (
        User user,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await GetAsync(user.Id, cancellationToken);

        var dto = await user.ToDtoAsync(variant, _fileUrlGenerator, cancellationToken);
        if (dto is null)
            return null;

        return new ReadDto<UserDto, UserPermission>
        (
            dto,
            permissions
        );

    }
    */

    public async ValueTask<(ImmutableHashSet<UserPermission> Permissions, UserVariant Variant)> GetAsync
    (
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var blockedByUserFact = await _facts.GetBlockedByUserFactAsync(userId, cancellationToken);
        var blockingUserFact = await _facts.GetBlockingUserFactAsync(userId, cancellationToken);
        var candidateUserFact = _facts.GetCandidateUserFact(userId);
        var sameUserFact = _facts.GetSameUserFact(userId);
        var matchedUserFact = await _facts.GetMatchedUserFactAsync(userId, cancellationToken);
        var isPositivelyJudgedByUserFact = await _facts.GetIsPositivelyJudgedByUserFactAsync(userId, cancellationToken);

        var permissions = GetPermissions(blockedByUserFact, blockingUserFact, candidateUserFact, sameUserFact, matchedUserFact, isPositivelyJudgedByUserFact);
        var variant = GetVariant(blockedByUserFact, blockingUserFact, candidateUserFact, sameUserFact, matchedUserFact, isPositivelyJudgedByUserFact);
        return (permissions, variant);
    }

    private static UserVariant GetVariant
    (
        BlockedByUserFact? isBlockedBy,
        BlockingUserFact? isBlocking,
        CandidateUserFact? isCandidate,
        SameUserFact? isSame,
        MatchedUserFact? isMatched,
        IsPositivelyJudged? isPositivelyJudgedByUserFact = null
    )
    {
        if (isBlockedBy?.Value == true || isBlocking?.Value == true)
        {
            return UserVariant.Blocked;
        }

        if (isSame?.Value == true)
        {
            return UserVariant.Full;
        }

        if (isMatched?.Value == true)
        {
            return UserVariant.Matched;
        }

        if (isCandidate?.Value == true)
        {
            return UserVariant.Candidate;
        }

        if (isPositivelyJudgedByUserFact?.Value == true)
        {
            return UserVariant.PositivelyJudged;
        }

        return UserVariant.Empty;
    }

    private static ImmutableHashSet<UserPermission> GetPermissions
    (
        BlockedByUserFact? isBlockedBy,
        BlockingUserFact? isBlocking,
        CandidateUserFact? isCandidate,
        SameUserFact? isSame,
        MatchedUserFact? isMatched,
        IsPositivelyJudged? isPositivelyJudgedByUserFact = null
    )
    {
        if (isBlockedBy?.Value == true || isBlocking?.Value == true)
        {
            return [];
        }

        var permissions = new HashSet<UserPermission>();
        if (isSame?.Value == true)
        {
            permissions.Add(UserPermission.View);
            permissions.Add(UserPermission.Update);
            permissions.Add(UserPermission.Delete);
        }

        if (isMatched?.Value == true)
        {
            permissions.Add(UserPermission.View);
        }

        if (isCandidate?.Value == true)
        {
            permissions.Add(UserPermission.View);
            permissions.Add(UserPermission.Judge);
        }

        if (isPositivelyJudgedByUserFact?.Value == true)
        {
            permissions.Add(UserPermission.View);
        }

        return [.. permissions];
    }

}

public enum UserPermission
{
    View = 0,
    Update = 1,
    Delete = 2,
    Judge = 3
}