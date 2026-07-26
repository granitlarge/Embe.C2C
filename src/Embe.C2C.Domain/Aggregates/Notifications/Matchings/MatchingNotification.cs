namespace Embe.C2C.Domain.Aggregates.Notifications.Matchings;

public abstract class MatchingNotification : Notification
{
    protected MatchingNotification
    (
        Guid recipientUserId,
        Guid matchingId,
        Guid partnerUserId
    ) : base(recipientUserId)
    {
        MatchingId = matchingId;
        PartnerUserId = partnerUserId;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected MatchingNotification()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {

    }

    public Guid MatchingId { get; }
    public Guid PartnerUserId { get; }
}