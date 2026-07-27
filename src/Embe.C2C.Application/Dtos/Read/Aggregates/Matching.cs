using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record MatchingDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    DateTimeOffset? CreatedAt,
    ReadDto<MessageDto, MessagePermission>? LastMessage,
    List<ReadDto<MessageDto, MessagePermission>>? Messages,
    ReadDto<UserDto, UserPermission>? User1,
    ReadDto<UserDto, UserPermission>? User2,
    ReadDto<SearchProfileDto, SearchProfilePermission>? User1SearchProfile,
    ReadDto<SearchProfileDto, SearchProfilePermission>? User2SearchProfile
);

public class MatchingDtoMapper
{
    private readonly MatchingAuthorizationService _matchingAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper;
    private readonly MessageDtoMapper _messageDtoMapper;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper;

    public MatchingDtoMapper
    (
        MatchingAuthorizationService matchingAuthorizationService,
        UserDtoMapper userDtoMapper,
        MessageDtoMapper messageDtoMapper,
        SearchProfileDtoMapper searchProfileDtoMapper
    )
    {
        _matchingAuthorizationService = matchingAuthorizationService;
        _userDtoMapper = userDtoMapper;
        _messageDtoMapper = messageDtoMapper;
        _searchProfileDtoMapper = searchProfileDtoMapper;
    }

    public async Task<ReadDto<MatchingDto, MatchingPermission>?> ToDtoAsync
    (
        Matching matching,
        User? queryingUser,
        CancellationToken cancellationToken
    )
    {

        var (permissions, variant) = _matchingAuthorizationService.Get(matching);
        if (!permissions.Contains(MatchingPermission.View))
        {
            return null;
        }

        var user1Dto = matching.User1 != null ? await _userDtoMapper.ToDtoAsync(matching.User1, queryingUser, cancellationToken) : null;
        var user2Dto = matching.User2 != null ? await _userDtoMapper.ToDtoAsync(matching.User2, queryingUser, cancellationToken) : null;

        var messageDtos = new List<ReadDto<MessageDto, MessagePermission>>();
        foreach (var message in matching.Messages ?? [])
        {
            var messageDto = await _messageDtoMapper.ToDtoAsync(message, cancellationToken);
            if (messageDto != null)
            {
                messageDtos.Add(messageDto);
            }
        }

        var lastMessageDto = matching.LastMessage != null ? await _messageDtoMapper.ToDtoAsync(matching.LastMessage, cancellationToken) : null;
        var user1SearchProfileDto = matching.User1SearchProfile != null ? await _searchProfileDtoMapper.ToDtoAsync(matching.User1SearchProfile, cancellationToken) : null;
        var user2SearchProfileDto = matching.User2SearchProfile != null ? await _searchProfileDtoMapper.ToDtoAsync(matching.User2SearchProfile, cancellationToken) : null;

        if (variant == MatchingVariant.Empty)
            return null;

        var dto = new MatchingDto
                (
                    matching.Id,
                    matching.UserId1,
                    matching.UserId2,
                    variant.IncludeCreatedAt ? matching.CreatedAt : null,
                    lastMessageDto,
                    messageDtos,
                    user1Dto,
                    user2Dto,
                    user1SearchProfileDto,
                    user2SearchProfileDto
                );

        return new ReadDto<MatchingDto, MatchingPermission>(dto, permissions);


    }
}