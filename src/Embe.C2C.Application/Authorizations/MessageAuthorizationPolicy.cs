using System.Collections.Immutable;
using Embe.C2C.Application.Authorizations.FactStores.Messages;
using Embe.C2C.Application.Authorizations.FactStores.Messages.Facts;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Messages;

namespace Embe.C2C.Application.Authorizations;

public class MessageAuthorizationPolicy
(
    MessageAuthorizationFactStore facts
)
{
    private readonly MessageAuthorizationFactStore _facts = facts;

    public async Task<ReadDto<MessageDto, MessagePermission>?> ToDtoAsync
    (
        Message message,
        CancellationToken cancellationToken
    )
    {
        var (permissions, variant) = await GetAsync(message, cancellationToken);
        if (!permissions.Contains(MessagePermission.View))
        {
            return null;
        }

        var messageReplyToDto = message.ReplyToMessage != null ? await ToDtoAsync(message.ReplyToMessage, cancellationToken) : null;
        var messageDto = message.ToDto(variant, messageReplyToDto);
        return new ReadDto<MessageDto, MessagePermission>(messageDto, permissions);
    }

    public async Task<ImmutableHashSet<MessagePermission>> GetPermissionsAsync
    (
        Guid messageId,
        CancellationToken cancellationToken
    )
    {
        var (permissions, _) = await GetAsync(messageId, cancellationToken);
        return permissions;
    }

    private async ValueTask<(ImmutableHashSet<MessagePermission> Permissions, MessageVariant Variant)> GetAsync
    (
        Message message,
        CancellationToken cancellationToken
    )
    {
        var authorFact = _facts.GetAuthorFact(message);
        var recipientFact = await _facts.GetRecipientFactAsync(message, cancellationToken);
        var permissions = GetPermissions(authorFact, recipientFact);
        var variant = authorFact.Value == true || recipientFact.Value == true ? MessageVariant.Full : MessageVariant.Empty;
        return (permissions, variant);
    }

    private async ValueTask<(ImmutableHashSet<MessagePermission> Permissions, MessageVariant Variant)> GetAsync
    (
        Guid messageId,
        CancellationToken cancellationToken
    )
    {
        var authorFact = await _facts.GetAuthorFactAsync(messageId, cancellationToken);
        var recipientFact = await _facts.GetRecipientFactAsync(messageId, cancellationToken);
        var permissions = GetPermissions(authorFact, recipientFact);
        var variant = authorFact.Value == true || recipientFact.Value == true ? MessageVariant.Full : MessageVariant.Empty;
        return (permissions, variant);
    }

    private static ImmutableHashSet<MessagePermission> GetPermissions
    (
        AuthorMessageFact authorFact,
        RecipientMessageFact recipientFact
    )
    {
        var permissions = new HashSet<MessagePermission>();
        if (authorFact.Value == true || recipientFact.Value == true)
        {
            permissions.Add(MessagePermission.View);
        }

        if (authorFact.Value == true)
        {
            permissions.Add(MessagePermission.Edit);
            permissions.Add(MessagePermission.Delete);
        }

        if (recipientFact.Value == true)
        {
            permissions.Add(MessagePermission.Report);
            permissions.Add(MessagePermission.Reply);
        }

        return [.. permissions];
    }
}

public enum MessagePermission
{
    View = 0,
    Edit = 1,
    Delete = 2,
    Report = 3,
    Reply = 4
}