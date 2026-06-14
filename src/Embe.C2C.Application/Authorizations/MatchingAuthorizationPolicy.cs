using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.Contexts;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations;

// 1) Only participants in a matching can view it.
// If a user has read access to a matching, we still must figure out
// if he has access to the related entities, and which level of access.

public class MatchingAuthorizationPolicy
{

    private readonly AuthorizationContext _authContext;
    private readonly MessageAuthorizationPolicy _messageAuthorizationPolicy;
    private readonly UserAuthorizationPolicy _userAuthorizationPolicy;
    private readonly IRepository _repo;
    private readonly IAuthenticatedUserService _authenticatedUser;

    public MatchingAuthorizationPolicy
    (
        AuthorizationContext authorizationContext,
        MessageAuthorizationPolicy messageAuthorizationPolicy,
        UserAuthorizationPolicy userAuthorizationPolicy,
        IRepository repo,
        IAuthenticatedUserService user
    )
    {
        _authContext = authorizationContext;
        _messageAuthorizationPolicy = messageAuthorizationPolicy;
        _userAuthorizationPolicy = userAuthorizationPolicy;
        _repo = repo;
        _authenticatedUser = user;
    }

    public IQueryable<Matching> GetViewable()
    {
        var userId = _authenticatedUser.UserId;
#warning if this changes, we'll have to make the update in two places, maybe we can unify the logic somehow?
        return _repo.MatchingsQuery.Where(m => m.UserId1 == userId || m.UserId2 == userId);
    }

    public async Task<ImmutableHashSet<MatchingPermission>> GetPermissionsAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        return GetPermissions(await GetMatchFactAsync(matchingId));
    }

    public async Task<ReadDto<MatchingDto, MatchingPermission>?> ToDtoAsync
    (
        Matching matching,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = Get(matching);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return null;
        }

        var messageDtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in matching.Conversation.Messages ?? [])
        {
            var messageDto = await _messageAuthorizationPolicy.ToDtoAsync(message, cancellationToken);
            if (messageDto != null)
            {
                messageDtos.Add(messageDto);
            }
        }

        var conversation = matching.Conversation.ToDto
        (
            ConversationVariant.Full,
            matching.Conversation?.LastMessage != null ? await _messageAuthorizationPolicy.ToDtoAsync(matching.Conversation.LastMessage, cancellationToken) : null,
            [.. messageDtos]
        );

        var user1 = matching.User1 != null ? await _userAuthorizationPolicy.ToDtoAsync(matching.User1, cancellationToken) : null;
        var user2 = matching.User2 != null ? await _userAuthorizationPolicy.ToDtoAsync(matching.User2, cancellationToken) : null;

        var matchingDto = matching.ToDto(variant, conversation, user1, user2);
        if (matchingDto == null)
        {
            return null;
        }
        return new ReadDto<MatchingDto, MatchingPermission>(matchingDto, permissions);
    }

    private (ImmutableHashSet<MatchingPermission> Permissions, MatchingVariant Variant) Get
    (
        Matching matching
    )
    {
        var fact = GetMatchFact(matching);
        var permissions = GetPermissions(fact);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return (permissions, MatchingVariant.Empty);
        }

        return (permissions, fact.IsParticipant ? MatchingVariant.Full : MatchingVariant.Empty);
    }

    private static ImmutableHashSet<MatchingPermission> GetPermissions
    (
        MatchFact fact
    )
    {
        var permissions = new HashSet<MatchingPermission>();
        if (fact.IsParticipant)
        {
            permissions.Add(MatchingPermission.View);
            permissions.Add(MatchingPermission.Unmatch);
        }

        return [.. permissions];
    }

    private async ValueTask<MatchFact> GetMatchFactAsync
    (
        Guid matchingId
    )
    {
        var cachedFacts = _authContext.Get<MatchFact>();
        var cachedFact = cachedFacts.FirstOrDefault(f => f.MatchId == matchingId);
        if (cachedFact != null)
        {
            return cachedFact;
        }

        var matching = await _repo.MatchingsQuery.AsNoTracking().SingleOrDefaultAsync(m => m.Id == matchingId);
        if (matching == null)
        {
            var notFoundFact = new MatchFact(matchingId, false);
            _authContext.Store(notFoundFact);
            return notFoundFact;
        }
        return GetMatchFact(matching);
    }

    private MatchFact GetMatchFact
    (
        Matching matching
    )
    {
        var cachedFacts = _authContext.Get<MatchFact>();
        var cachedFact = cachedFacts.FirstOrDefault(f => f.MatchId == matching.Id);
        if (cachedFact != null)
        {
            return cachedFact;
        }

        var userId = _authenticatedUser.UserId;
        var fact = new MatchFact(matching.Id, matching.UserId1 == userId || matching.UserId2 == userId);
        _authContext.Store(fact);
        return fact;
    }

}

public enum MatchingPermission
{
    View,
    Unmatch
}