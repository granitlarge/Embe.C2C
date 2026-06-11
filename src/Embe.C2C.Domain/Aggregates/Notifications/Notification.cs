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

    protected Notification()
    {

    }

    public Guid Id { get; }
    public Guid RecipientUserId { get; }
    public bool IsRead => ReadAt.HasValue;

    public DateTimeOffset? ReadAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt => ReadAt ?? CreatedAt;

    public void MarkAsRead(bool isRead)
    {
        if (isRead == IsRead)
        {
            return;
        }

        ReadAt = isRead ? DateTimeOffset.UtcNow : null;
        AddDomainEvent(new NotificationUpdatedEvent(this));
    }

    public void Remove()
    {
        AddDomainEvent(new NotificationRemovedEvent(this));
    }
}