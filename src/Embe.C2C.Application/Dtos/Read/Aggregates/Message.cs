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
    private readonly MessageAuthorizationService _messageAuthorizationService;

    public MessageDtoMapper(MessageAuthorizationService messageAuthorizationService)
    {
        _messageAuthorizationService = messageAuthorizationService;
    }

    public async Task<ReadDto<MessageDto, MessagePermission>?> ToDtoAsync
    (
        Message message,
        CancellationToken cancellationToken
    )
    {

        var (permissions, variant) = await _messageAuthorizationService.GetAsync(message, cancellationToken);
        if (!permissions.Contains(MessagePermission.View))
        {
            return null;
        }

        var replyDto = message.ReplyToMessage != null ? await ToDtoAsync(message.ReplyToMessage, cancellationToken) : null;
        if (variant == MessageVariant.Empty)
        {
            return null;
        }

        var dto = new MessageDto
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
            variant.IncludeReplyToMessage ? replyDto : null
        );

        return new ReadDto<MessageDto, MessagePermission>(dto, permissions);

    }
}