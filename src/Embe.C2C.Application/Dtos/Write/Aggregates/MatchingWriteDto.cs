using Embe.C2C.Application.Dtos.Write.Entities;

namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record MatchingWriteDto
(
    Guid Id,
    Guid UserId1,
    Guid UserId2,
    ConversationWriteDto? Conversation,
    DateTimeOffset? CreatedAt,
    UserWriteDto? User1,
    UserWriteDto? User2
);