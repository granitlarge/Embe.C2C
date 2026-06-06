using Embe.C2C.Domain.Aggregates.Messages;

namespace Embe.C2C.Application.Dtos.Aggregates;

public record MessageDto
(
    Guid Id,
    Guid ConversationId,
    Guid? ReplyToMessageId,
    Guid AuthorUserId,
    string Content,
    DateTimeOffset? SeenAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset EditedAt
);

public static class MessageDtoExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        (
            message.Id,
            message.ConversationId,
            message.ReplyToMessageId,
            message.AuthorUserId,
            message.Content.Value,
            message.SeenAt,
            message.CreatedAt,
            message.EditedAt
        );
    }
}
