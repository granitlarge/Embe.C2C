using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class MatchingExtensions
{
    public static async Task<ReadDto<MatchingDto, MatchingPermission>?> ToDtoAsync
    (
        this Matching matching,
        User? queryingUser,
        User? user1,
        User? user2,
        MatchingAuthorizationService matchingAuthorizationService,
        MatchingDtoMapper matchingDtoMapper,
        UserAuthorizationService userAuthorizationService,
        UserDtoMapper userDtoMapper,
        MessageAuthorizationService messageAuthorizationService,
        MessageDtoMapper messageDtoMapper,
        SearchProfileAuthorizationService searchProfileAuthorizationService,
        SearchProfileDtoMapper searchProfileDtoMapper,
        CancellationToken cancellationToken = default
    )
    {
        var (matchingPermissions, matchingVariant) = matchingAuthorizationService.Get(matching);
        if (!matchingPermissions.Contains(MatchingPermission.View))
        {
            return null;
        }

        var enrichedUser1 = user1?.Enrich(queryingUser);
        var enrichedUser2 = user2?.Enrich(queryingUser);
        var user1Dto = await (enrichedUser1?.ToDtoAsync(userAuthorizationService, userDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<UserDto, UserPermission>?>(null));
        var user2Dto = await (enrichedUser2?.ToDtoAsync(userAuthorizationService, userDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<UserDto, UserPermission>?>(null));

        var messageDtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in matching.Messages ?? [])
        {
            var messageDto = await message.ToDtoAsync(messageAuthorizationService, messageDtoMapper, cancellationToken);
            if (messageDto != null)
            {
                messageDtos.Add(messageDto);
            }
        }

        var lastMessageDto = await (matching.LastMessage?.ToDtoAsync(messageAuthorizationService, messageDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<MessageDto, MessagePermission>?>(null));
        var user1SearchProfileDto = await (matching.User1SearchProfile?.ToDtoAsync(searchProfileAuthorizationService, searchProfileDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<SearchProfileDto, SearchProfilePermission>?>(null));
        var user2SearchProfileDto = await (matching.User2SearchProfile?.ToDtoAsync(searchProfileAuthorizationService, searchProfileDtoMapper, cancellationToken) ?? Task.FromResult<ReadDto<SearchProfileDto, SearchProfilePermission>?>(null));
        var matchingDto = matchingDtoMapper.ToDto
        (
            matching,
            matchingVariant,
            lastMessageDto,
            messageDtos,
            user1: user1Dto,
            user2: user2Dto,
            user1SearchProfile: user1SearchProfileDto,
            user2SearchProfile: user2SearchProfileDto
        );

        return matchingDto != null ? new ReadDto<MatchingDto, MatchingPermission>(matchingDto, matchingPermissions) : null;
    }
}