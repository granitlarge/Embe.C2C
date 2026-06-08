using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Entities;

public class Conversation : Entity
{
    private Conversation
    (
        Guid matchingId,
        Guid userId1,
        Guid userId2
    )
    {
        if (userId1 == userId2)
        {
            throw new DomainException("A conversation cannot be created between the same user.");
        }

        Id = Guid.CreateVersion7();
        MatchingId = matchingId;
        UserId1 = userId1;
        UserId2 = userId2;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; }
    public Guid MatchingId { get; }
    public Guid UserId1 { get; private set; }
    public Guid UserId2 { get; private set; }
    public Guid? LastMessageId { get; private set; }
    public uint MessageCount { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    internal void IncrementMessageCount()
    {
        MessageCount++;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    internal void DecrementMessageCount()
    {
        if (MessageCount == 0)
        {
            throw new DomainException("Message count cannot be negative.");
        }

        MessageCount--;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    internal void UpdateLastMessageId(Guid messageId)
    {
        LastMessageId = messageId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    internal static Conversation Create
    (
        Guid matchingId,
        Guid userId1,
        Guid userId2
    )
    {
        return new Conversation(matchingId, userId1, userId2);
    }
}