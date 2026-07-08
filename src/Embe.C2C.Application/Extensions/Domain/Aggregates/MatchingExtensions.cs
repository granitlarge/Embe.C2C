using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class MatchingExtensions
{
    public static async Task<ReadDto<MatchingDto, MatchingPermission>?> ToDtoAsync
    (
        this Matching matching,
        User? user1,
        User? user2,
        MatchingAuthorizationService matchingAuthorizationService,
        MatchingDtoMapper matchingDtoMapper,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        MessageAuthorizationService messageAuthorizationService,
        MessageDtoMapper messageDtoMapper,
        ConversationDtoMapper conversationDtoMapper,
        CancellationToken cancellationToken = default
    )
    {
        var (matchingPermissions, matchingVariant) = matchingAuthorizationService.Get(matching);
        if (!matchingPermissions.Contains(MatchingPermission.View))
        {
            return null;
        }

        var user1Dto = await (user1?.ToDtoAsync(userAuthorizationService, userDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<UserDto, UserPermission>?>(null));
        var user2Dto = await (user2?.ToDtoAsync(userAuthorizationService, userDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<UserDto, UserPermission>?>(null));

        var messageDtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in matching.Conversation.Messages ?? [])
        {
            var messageDto = await message.ToDtoAsync(messageAuthorizationService, messageDtoMapper, cancellationToken);
            if (messageDto != null)
            {
                messageDtos.Add(messageDto);
            }
        }

        var lastMessageDto = await (matching.Conversation.LastMessage?.ToDtoAsync(messageAuthorizationService, messageDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<MessageDto, MessagePermission>?>(null));
        var conversation = conversationDtoMapper.ToDto
        (
            matching.Conversation,
            ConversationVariant.Full,
            lastMessageDto,
            [.. messageDtos]
        );

        var matchingDto = matchingDtoMapper.ToDto
        (
            matching,
            matchingVariant,
            conversation: conversation,
            user1: user1Dto,
            user2: user2Dto
        );

        return matchingDto != null ? new ReadDto<MatchingDto, MatchingPermission>(matchingDto, matchingPermissions) : null;
    }
}