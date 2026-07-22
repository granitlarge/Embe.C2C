using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Messages;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public record MessageDto
(
    Guid Id,
    Guid ConversationId,
    Guid? ReplyToMessageId,
    Guid AuthorUserId,
    bool? IsReply,
    string? Content,
    DateTimeOffset? SeenAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? EditedAt,
    ReadDto<MessageDto, MessagePermission>? ReplyToMessage
);

public class MessageDtoMapper
{
    public MessageDtoMapper()
    {

    }

    public MessageDto? ToDto
    (
        Message message,
        MessageVariant variant,
        ReadDto<MessageDto, MessagePermission>? replyToMessageDto = null
    )
    {
        if (variant == MessageVariant.Empty)
            return null;

        return new MessageDto
        (
            message.Id,
            message.MatchingId,
            message.ReplyToMessageId,
            message.AuthorUserId,
            variant.IncludeIsReply ? message.IsReply : null,
            variant.IncludeContent ? message.Content?.Value : null,
            variant.IncludeSeenAt ? message.SeenAt : null,
            variant.IncludeCreatedAt ? message.CreatedAt : null,
            variant.IncludeEditedAt ? message.EditedAt : null,
            variant.IncludeReplyToMessage ? replyToMessageDto : null
        );
    }
}