using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

namespace Embe.C2C.Application.Dtos.Read.Entities;

public record ConversationDto
(
    Guid Id,
    Guid MatchingId,
    Guid UserId1,
    Guid UserId2,
    Guid? LastMessageId,
    uint? MessageCount,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? CreatedAt,
    ReadDto<MessageDto, MessagePermission>? LastMessage,
    ReadDto<MessageDto, MessagePermission>[]? Messages
);

public class ConversationDtoMapper
{
    public ConversationDtoMapper()
    {

    }

    public ConversationDto? ToDto
    (
        Domain.Entities.Conversation conversation,
        ConversationVariant variant,
        ReadDto<MessageDto, MessagePermission>? lastMessage,
        ReadDto<MessageDto, MessagePermission>[]? messages
    )
    {
        if (variant == ConversationVariant.Empty)
            return null;

        return new ConversationDto
        (
            conversation.Id,
            conversation.MatchingId,
            conversation.UserId1,
            conversation.UserId2,
            conversation.LastMessageId,
            variant.IncludeMessageCount ? conversation.MessageCount : null,
            variant.IncludeUpdatedAt ? conversation.UpdatedAt : null,
            variant.IncludeCreatedAt ? conversation.CreatedAt : null,
            lastMessage,
            messages
        );

    }
}