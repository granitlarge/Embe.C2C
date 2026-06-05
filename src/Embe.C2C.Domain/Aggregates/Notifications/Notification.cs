using Embe.C2C.Domain.Aggregates.Notifications.Events;
namespace Embe.C2C.Domain.Aggregates.Notifications;

public abstract class Notification : Aggregate
{
    protected Notification
    (
        Guid recipientUserId
    )
    {
        Id = Guid.CreateVersion7();
        RecipientUserId = recipientUserId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }
    public Guid RecipientUserId { get; }
    public bool IsRead => ReadAt.HasValue;

    public DateTimeOffset? ReadAt { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public void MarkAsRead()
    {
        if (ReadAt.HasValue)
        {
            return;
        }

        ReadAt = DateTimeOffset.UtcNow;
    }

    public void Remove()
    {
        AddDomainEvent(new NotificationRemovedEvent(this));
    }
}