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
            throw new DomainException("You cannot send a message to this user.");
        }

        var conversation = matching.Conversation;
        var message = Message.Create(conversation.Id, replyToMessage?.Id, author.Id, content);
        AddDomainEvent(new MessageSentEvent(message));
        conversation.UpdateLastMessageId(message.Id);
        conversation.IncrementMessageCount();

        return message;
    }

    public void EditMessage(User editor, Message message, MessageContent newContent)
    {
        if (message.AuthorUserId != editor.Id)
        {
            throw new DomainException("Users can only edit their own messages.");
        }

        message.Edit(newContent);
    }

    public void DeleteMessage(User deleter, Message message, Matching matching)
    {
        if (message.AuthorUserId != deleter.Id)
        {
            throw new DomainException("Users can only delete their own messages.");
        }

        var conversation = matching.Conversation;
        conversation.DecrementMessageCount();
    }
}