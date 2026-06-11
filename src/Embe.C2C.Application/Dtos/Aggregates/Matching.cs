using Embe.C2C.Application.Dtos.Entities;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Dtos.Aggregates;

public record MatchingDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    ConversationDto Conversation,
    DateTimeOffset CreatedAt,
    UserBriefDto? User
);

public static class MatchingDtoExtensions
{
    public static async Task<MatchingDto> ToDto(this Matching matching, IFileUrlGenerator fileUrlGenerator)
    {
        return new MatchingDto
        (
            matching.Id,
            matching.UserId1,
            matching.UserId2,
            matching.Conversation.ToDto(),
            matching.CreatedAt,

            matching.User1 != null ? await matching.User1.ToBriefDto(fileUrlGenerator) :
            matching.User2 != null ? await matching.User2.ToBriefDto(fileUrlGenerator) :
            null
        );
    }
}
