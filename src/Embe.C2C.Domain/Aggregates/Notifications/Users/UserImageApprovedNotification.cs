namespace Embe.C2C.Domain.Aggregates.Notifications.Users;

public class UserImageApprovedNotification : Notification
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public UserImageApprovedNotification()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public UserImageApprovedNotification
    (
        Guid recipientUserId,
        Guid imageId,
        string originalImageUrl,
        string largeImageUrl,
        string mediumImageUrl,
        string smallImageUrl
    ) : base(recipientUserId)
    {
        ImageId = imageId;
        OriginalImageUrl = originalImageUrl;
        LargeImageUrl = largeImageUrl;
        MediumImageUrl = mediumImageUrl;
        SmallImageUrl = smallImageUrl;
    }

    public Guid ImageId { get; private set; }
    public string OriginalImageUrl { get; private set; }
    public string LargeImageUrl { get; private set; }
    public string MediumImageUrl { get; private set; }
    public string SmallImageUrl { get; private set; }
}