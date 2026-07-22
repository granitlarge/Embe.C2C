namespace Embe.C2C.Domain.Aggregates.Notifications.Users;

public class UserImageRejectedNotification : Notification
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public UserImageRejectedNotification()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public UserImageRejectedNotification
    (
        Guid recipientUserId,
        Guid imageId
    ) : base(recipientUserId)
    {
        ImageId = imageId;
    }

    public Guid ImageId { get; private set; }
}