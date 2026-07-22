using Embe.C2C.Domain.Aggregates.Messages.Events;
using Embe.C2C.Domain.Entities;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Messages;

public class Message : Aggregate
{
    private Message
    (
        Guid matchingId,
        Guid? replyToMessageId,
        Guid authorUserId,
        MessageContent content
    )
    {
        Id = Guid.CreateVersion7();
        MatchingId = matchingId;
        ReplyToMessageId = replyToMessageId;
        IsReply = replyToMessageId.HasValue;
        AuthorUserId = authorUserId;
        Content = content;
        CreatedAt = DateTimeOffset.UtcNow;
        EditedAt = CreatedAt;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Message()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid Id { get; }
    public Guid MatchingId { get; }
    public Guid? ReplyToMessageId { get; private set; }
    public bool IsReply { get; private set; }
    public Guid AuthorUserId { get; }
    public MessageContent Content { get; private set; }
    public DateTimeOffset? SeenAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset EditedAt { get; private set; }

    internal void Edit(MessageContent newContent)
    {
        Content = newContent;
        EditedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsSeen(bool seen)
    {
        if (seen && SeenAt.HasValue)
        {
            return;
        }

        if (seen)
        {
            AddDomainEvent(new MessageSeenEvent(this));
        }
        else
        {
            AddDomainEvent(new MessageUnseenEvent(this));
        }

        SeenAt = seen ? DateTimeOffset.UtcNow : null;
    }

    internal void Remove()
    {
        AddDomainEvent(new MessageRemovedEvent(this));
    }

    internal static Message Create(Guid matchingId, Guid? replyToMessageId, Guid authorUserId, MessageContent content)
    {
        return new Message(matchingId, replyToMessageId, authorUserId, content);
    }

    internal void ReplyMessageRemoved()
    {
        ReplyToMessageId = null;
    }

    #region Read Only Navigation Properties
    public Matchings.Matching? Matching { get; private set; }
    public Message? ReplyToMessage { get; private set; }
    #endregion
}

public enum MessageError
{
    CannotCommunicate,
    Unauthorized,
    InvalidReply
}