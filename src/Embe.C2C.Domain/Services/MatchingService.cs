using Embe.C2C.Domain.Aggregates.Matchings;
using Embe.C2C.Domain.Aggregates.Messages;
using Embe.C2C.Domain.Aggregates.Messages.Events;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.Policies;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Services;

public class MatchingService : DomainService
{
    private readonly DomainEventStore _domainEventStore;

    public MatchingService(DomainEventStore domainEventStore)
    {
        _domainEventStore = domainEventStore;
    }

    public Message SendMessage
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
            throw new DomainException(new DomainError<MessageError>(MessageError.CannotCommunicate));
        }

        var conversation = matching.Conversation;
        var message = Message.Create(conversation.Id, replyToMessage?.Id, author.Id, content);
        _domainEventStore.AddDomainEvent(new MessageCreatedEvent(message));
        conversation.UpdateLastMessageId(message.Id);
        conversation.IncrementMessageCount();

        return message;
    }

    public void EditMessage(User editor, Message message, MessageContent newContent)
    {
        if (message.AuthorUserId != editor.Id)
        {
            throw new DomainException(new DomainError<MessageError>(MessageError.Unauthorized));
        }

        message.Edit(newContent);
        _domainEventStore.AddDomainEvent(new MessageEditedEvent(message));
    }

    public void DeleteMessage
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
            throw new DomainException(new DomainError<MessageError>(MessageError.Unauthorized));
        }

        foreach (var reply in replies)
        {
            if (reply.ReplyToMessageId != message.Id)
            {
                throw new DomainException(new DomainError<MessageError>(MessageError.InvalidReply));
            }
        }

        var conversation = matching.Conversation;
        conversation.DecrementMessageCount();
        conversation.UpdateLastMessageId(newLastMessage?.Id);
        message.Remove();

        foreach (var reply in replies)
        {
            reply.ReplyMessageRemoved();
        }

        _domainEventStore.AddDomainEvent(new MessageRemovedEvent(message));
    }
}