namespace Embe.C2C.Domain.Aggregates.Notifications.Messages;

public abstract class MessageNotification
(
    Guid messageId,
    Guid recipientUserId
) : Notification(recipientUserId)
{
    public Guid MessageId { get; private set; } = messageId;
}