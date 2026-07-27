namespace Embe.C2C.Domain.Aggregates.Notifications.Messages;

public class MessageCreatedNotification : MessageNotification
{
    private MessageCreatedNotification(Guid messageId, Guid recipientUserId) : base(messageId, recipientUserId)
    {

    }

    public static MessageNotification Create(Guid messageId, Guid recipientUserId)
    {
        return new MessageCreatedNotification(messageId, recipientUserId);
    }
}