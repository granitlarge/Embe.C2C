namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record MessageWriteDto
(
    Guid Id,
    Guid ConversationId,
    Guid? ReplyToMessageId,
    Guid AuthorUserId,
    string? Content,
    DateTimeOffset? SeenAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? EditedAt
);