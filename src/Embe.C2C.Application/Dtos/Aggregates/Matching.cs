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
    public static async Task<MatchingDto> ToDtoAsync(this Matching matching, Guid userId, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        return new MatchingDto
        (
            matching.Id,
            matching.UserId1,
            matching.UserId2,
            matching.Conversation.ToDto(),
            matching.CreatedAt,

            matching.User1 != null && matching.User1.Id == userId ? await matching.User1.ToBriefDtoAsync(fileUrlGenerator, cancellationToken) :
            matching.User2 != null && matching.User2.Id == userId ? await matching.User2.ToBriefDtoAsync(fileUrlGenerator, cancellationToken) :
            null
        );
    }
}
