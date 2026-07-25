using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Messages.Events;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Policies;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Domain.Services;

public class MatchingService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public MatchingService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

    public ErrorOr<Message> SendMessage
    (
        User author,
        Matching matching,
        MessageContent content,
        CommunicationPolicy communicationPolicy,
        Message? replyToMessage = null
    )
    {
        if (!communicationPolicy.CanCommunicate())
        {
            return DomainErrors.MatchingSendMessageCannotCommunicate.ToValidationErrorOr(new Dictionary<string, object>
            {
                { "authorId", author.Id },
                { "recipientId", matching.GetOtherUserId(author.Id)! }
            });
        }

        var message = Message.Create(matching.Id, replyToMessage?.Id, author.Id, content);
        _domainEventStore.AddDomainEvent(new MessageCreatedEvent(message));
        matching.UpdateLastMessageId(message.Id);

        return message;
    }

    public ErrorOr<Message> EditMessage(User editor, Message message, MessageContent newContent)
    {
        if (message.AuthorUserId != editor.Id)
        {
            return DomainErrors.Forbidden.ToValidationErrorOr(new Dictionary<string, object>
            {
                { "editorId", editor.Id },
                { "messageId", message.Id }
            });
        }

        message.Edit(newContent);
        _domainEventStore.AddDomainEvent(new MessageEditedEvent(message));
        return message;
    }

    public ErrorOr<Message> DeleteMessage
    (
        User deleter,
        Message message,
        Message? newLastMessage,
        Matching matching,
        List<Message> replies
    )
    {
        if (message.AuthorUserId != deleter.Id)
        {
            return DomainErrors.Forbidden.ToValidationErrorOr(new Dictionary<string, object>
            {
                { "deleterId", deleter.Id },
                { "messageId", message.Id }
            });
        }

        foreach (var reply in replies)
        {
            if (reply.ReplyToMessageId != message.Id)
            {
                return DomainErrors.MessageInvalidReply.ToValidationErrorOr(new Dictionary<string, object>
                {
                    { "messageId", message.Id }
                });
            }
        }

        matching.UpdateLastMessageId(newLastMessage?.Id);
        message.Remove();

        foreach (var reply in replies)
        {
            reply.ReplyMessageRemoved();
        }

        _domainEventStore.AddDomainEvent(new MessageRemovedEvent(message));
        return message;
    }
}