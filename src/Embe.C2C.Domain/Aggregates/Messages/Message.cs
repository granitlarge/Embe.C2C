using Embe.C2C.Domain.Aggregates.Messages.Events;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Messages;

public class Message : Aggregate
{
    private Message
    (
        Guid conversationId,
        Guid? replyToMessageId,
        Guid authorUserId,
        MessageContent content
    )
    {
        Id = Guid.CreateVersion7();
        ConversationId = conversationId;
        ReplyToMessageId = replyToMessageId;
        AuthorUserId = authorUserId;
        Content = content;
        CreatedAt = DateTimeOffset.UtcNow;
        EditedAt = CreatedAt;
    }

    public Guid Id { get; }
    public Guid ConversationId { get; }
    public Guid? ReplyToMessageId { get; }
    public Guid AuthorUserId { get; }
    public MessageContent Content { get; private set; }
    public DateTimeOffset? SeenAt { get; private set; }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset EditedAt { get; private set; }

    internal void Edit(MessageContent newContent)
    {
        Content = newContent;
        EditedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsSeen()
    {
        SeenAt = DateTimeOffset.UtcNow;
    }

    public void Remove()
    {
        AddDomainEvent(new MessageRemovedEvent(this));
    }

    internal static Message Create(Guid conversationId, Guid? replyToMessageId, Guid authorUserId, MessageContent content)
    {
        return new Message(conversationId, replyToMessageId, authorUserId, content);
    }
}