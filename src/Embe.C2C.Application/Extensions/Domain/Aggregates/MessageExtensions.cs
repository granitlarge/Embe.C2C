using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class MessageExtensions
{
    public static async Task<ReadDto<MessageDto, MessagePermission>?> ToDtoAsync
    (
        this Message message,
        MessageAuthorizationService messageAuthorizationService,
        MessageDtoMapper messageDtoMapper,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await messageAuthorizationService.GetAsync(message, cancellationToken);
        if (!permissions.Contains(MessagePermission.View))
        {
            return null;
        }
        var (replyPermissions, replyVariant) = message.ReplyToMessage != null ? await messageAuthorizationService.GetAsync(message.ReplyToMessage, cancellationToken) : (ImmutableHashSet<MessagePermission>.Empty, MessageVariant.Empty);
        var replyDto = message.ReplyToMessage != null ? messageDtoMapper.ToDto(message.ReplyToMessage, replyVariant) : null;
        var messageDto = messageDtoMapper.ToDto(message, variant, replyDto != null ? new ReadDto<MessageDto, MessagePermission>(replyDto, replyPermissions) : null);
        return messageDto != null ? new ReadDto<MessageDto, MessagePermission>(messageDto, permissions) : null;
    }
}