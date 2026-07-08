using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Authorizations.FactStores.Matches;
using Embe.C2C.Application.Authorizations.FactStores.Matches.Facts;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Authorizations;

public class MatchingAuthorizationService
{
    private readonly IRepository _repo;
    private readonly MatchingAuthorizationFactStore _facts;
    private readonly MessageAuthorizationService _messageAuthorizationPolicy;
    private readonly UserAuthorizationService _userAuthorizationPolicy;

    public MatchingAuthorizationService
    (
        IRepository repo,
        MatchingAuthorizationFactStore facts,
        MessageAuthorizationService messageAuthorizationPolicy,
        UserAuthorizationService userAuthorizationPolicy
    )
    {
        _repo = repo;
        _facts = facts;
        _messageAuthorizationPolicy = messageAuthorizationPolicy;
        _userAuthorizationPolicy = userAuthorizationPolicy;
    }

    public IQueryable<Matching> GetViewable()
    {
        var userId = _facts.CurrentUserId;
#warning if this changes, we'll have to make the update in two places, maybe we can unify the logic somehow?
        return _repo.MatchingsQuery.Where(m => m.UserId1 == userId || m.UserId2 == userId);
    }

    public async Task<ImmutableHashSet<MatchingPermission>> GetPermissionsAsync(Guid matchingId, CancellationToken cancellationToken)
    {
        return GetPermissions(await _facts.GetIsParticipantFactAsync(matchingId, cancellationToken));
    }
/*
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
*/
    public (ImmutableHashSet<MatchingPermission> Permissions, MatchingVariant Variant) Get
    (
        Matching matching
    )
    {
        var isParticipantFact = _facts.GetIsParticipantFact(matching);
        var permissions = GetPermissions(isParticipantFact);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return (permissions, MatchingVariant.Empty);
        }

        return (permissions, isParticipantFact.Value ? MatchingVariant.Full : MatchingVariant.Empty);
    }

    private static ImmutableHashSet<MatchingPermission> GetPermissions
    (
        IsParticipantMatchFact fact
    )
    {
        var permissions = new HashSet<MatchingPermission>();
        if (fact.Value)
        {
            permissions.Add(MatchingPermission.View);
            permissions.Add(MatchingPermission.Unmatch);
            permissions.Add(MatchingPermission.Chat);
        }

        return [.. permissions];
    }

}

public enum MatchingPermission
{
    View = 0,
    Unmatch = 1,
    Chat = 2
}