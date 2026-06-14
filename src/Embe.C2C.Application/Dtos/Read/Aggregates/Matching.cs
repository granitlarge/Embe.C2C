using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record MatchingDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    ConversationDto? Conversation,
    DateTimeOffset? CreatedAt,
    ReadDto<UserDto, UserPermission>? User1,
    ReadDto<UserDto, UserPermission>? User2
);

public static class MatchingDtoExtensions
{
    public static MatchingDto? ToDto
    (
        this Matching matching,
        MatchingVariant variant,
        ConversationDto? conversation,
        ReadDto<UserDto, UserPermission>? user1,
        ReadDto<UserDto, UserPermission>? user2
    )
    {
        if (variant == MatchingVariant.Empty)
        {
            return null;
        }

        return new MatchingDto
        (
            matching.Id,
            matching.UserId1,
            matching.UserId2,
            conversation,
            matching.CreatedAt,
            user1,
            user2
        );
    }
}