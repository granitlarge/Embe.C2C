using Embe.C2C.Domain.Aggregates.Blockings.Events;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Aggregates.Blockings;

public class Blocking : Aggregate
{
    public Blocking
    (
        Guid blockerUserId,
        Guid blockedUserId
    )
    {
        if (blockerUserId == blockedUserId)
        {
            throw new DomainException(new DomainError<BlockingError>(BlockingError.SelfBlock));
        }

        Id = Guid.CreateVersion7();
        BlockerUserId = blockerUserId;
        BlockedUserId = blockedUserId;
        BlockedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new BlockingCreatedEvent(this));
    }

    private Blocking() { }

    public Guid Id { get; }
    public Guid BlockerUserId { get; }
    public Guid BlockedUserId { get; }
    public DateTimeOffset BlockedAt { get; private set; }

    public void Remove()
    {
        AddDomainEvent(new BlockingRemovedEvent(this));
    }

    internal static Blocking Create(Guid blockerUserId, Guid blockedUserId)
    {
        return new Blocking(blockerUserId, blockedUserId);
    }
}

public enum BlockingError
{
    SelfBlock
}