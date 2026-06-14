using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Messages;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record MessageDto
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

public static class MessageDtoExtensions
{
    public static MessageDto ToDto(this Message message, MessageVariant variant)
    {
        return new MessageDto
        (
            message.Id,
            message.ConversationId,
            message.ReplyToMessageId,
            message.AuthorUserId,
            variant.IncludeContent ? message.Content?.Value : null,
            variant.IncludeSeenAt ? message.SeenAt : null,
            variant.IncludeCreatedAt ? message.CreatedAt : null,
            variant.IncludeEditedAt ? message.EditedAt : null
        );
    }
}
