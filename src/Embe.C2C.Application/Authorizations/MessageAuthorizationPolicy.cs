using System.Collections.Immutable;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations.Contexts;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Messages;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Authorizations;

public class MessageAuthorizationPolicy
{
    private readonly AuthorizationContext _context;
    private readonly IRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUser;

    public MessageAuthorizationPolicy
    (
        AuthorizationContext context,
        IRepository repository,
        IAuthenticatedUserService authenticatedUser
    )
    {
        _context = context;
        _repository = repository;
        _authenticatedUser = authenticatedUser;
    }

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

        var messageDto = message.ToDto(variant);
        return new ReadDto<MessageDto, MessagePermission>(messageDto, permissions);
    }

    private async ValueTask<(ImmutableHashSet<MessagePermission> Permissions, MessageVariant Variant)> GetAsync
    (
        Message message,
        CancellationToken cancellationToken
    )
    {
        var fact = await GetFactAsync(message, cancellationToken);
        var permissions = GetPermissions(fact);
        var variant = fact.IsAuthor || fact.IsRecipient ? MessageVariant.Full : MessageVariant.Empty;
        return (permissions, variant);
    }

    private async ValueTask<MessageFact> GetFactAsync
    (
        Message message,
        CancellationToken cancellationToken
    )
    {
        var messageFact = _context.Get<MessageFact>(message.Id);
        if (messageFact != null)
        {
            return messageFact;
        }

        bool isAuthor = message.AuthorUserId == _authenticatedUser.UserId;
        if (isAuthor)
        {
            var fact = new MessageFact(message.Id, true, false);
            _context.Store(fact);
            return fact;
        }
        else
        {
            bool isRecipient = false;
            var conversationFact = _context.Get<ConversationFact>(message.ConversationId);
            if (conversationFact != null)
            {
                isRecipient = conversationFact.IsParticipant;
            }
            else
            {
                isRecipient = await _repository
                    .MessagesQuery
                    .AnyAsync(m => m.Id == message.Id && (m.Conversation!.UserId1 == _authenticatedUser.UserId || m.Conversation.UserId2 == _authenticatedUser.UserId), cancellationToken);
                _context.Store(new ConversationFact(message.ConversationId, isRecipient));
            }

            var fact = new MessageFact(message.Id, isAuthor, isRecipient);
            _context.Store(fact);
            return fact;
        }
    }

    private static ImmutableHashSet<MessagePermission> GetPermissions
    (
        MessageFact fact
    )
    {
        var permissions = new HashSet<MessagePermission>();
        if (fact.IsAuthor || fact.IsRecipient)
        {
            permissions.Add(MessagePermission.View);
        }
        if (fact.IsAuthor)
        {
            permissions.Add(MessagePermission.Edit);
            permissions.Add(MessagePermission.Delete);
        }
        return [.. permissions];
    }
}

public enum MessagePermission
{
    View = 0,
    Edit = 1,
    Delete = 2
}