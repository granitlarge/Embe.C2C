using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;

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
    public MatchingDtoMapper()
    {

    }

    public MatchingDto? ToDto
    (
        Matching matching,
        MatchingVariant variant,
        ReadDto<MessageDto, MessagePermission>? lastMessage,
        List<ReadDto<MessageDto, MessagePermission>>? messages,
        ReadDto<UserDto, UserPermission>? user1,
        ReadDto<UserDto, UserPermission>? user2,
        ReadDto<SearchProfileDto, SearchProfilePermission>? user1SearchProfile,
        ReadDto<SearchProfileDto, SearchProfilePermission>? user2SearchProfile
    )
    {
        if (variant == MatchingVariant.Empty)
            return null;

        return new MatchingDto
        (
            matching.Id,
            matching.UserId1,
            matching.UserId2,
            variant.IncludeCreatedAt ? matching.CreatedAt : null,
            lastMessage,
            messages,
            user1,
            user2,
            user1SearchProfile,
            user2SearchProfile
        );

    }
}