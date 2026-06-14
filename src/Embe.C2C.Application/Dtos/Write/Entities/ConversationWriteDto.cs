using Embe.C2C.Application.Dtos.Write.Aggregates;

namespace Embe.C2C.Application.Dtos.Write.Entities;

public record ConversationWriteDto
(
    Guid Id,
    Guid MatchingId,
    Guid UserId1,
    Guid UserId2,
    Guid? LastMessageId,
    uint? MessageCount,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? CreatedAt,
    MessageWriteDto? LastMessage,
    MessageWriteDto[]? Messages
);