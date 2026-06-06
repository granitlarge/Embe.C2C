using Embe.C2C.Application.Dtos.Entities;
using Embe.C2C.Domain.Aggregates.Matchings;

namespace Embe.C2C.Application.Dtos.Aggregates;

public record MatchingDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    ConversationDto Conversation,
    DateTimeOffset CreatedAt
);

public static class MatchingDtoExtensions
{
    public static MatchingDto ToDto(this Matching matching)
    {
        return new MatchingDto
        (
            matching.Id,
            matching.UserId1,
            matching.UserId2,
            matching.Conversation.ToDto(),
            matching.CreatedAt
        );
    }
}
