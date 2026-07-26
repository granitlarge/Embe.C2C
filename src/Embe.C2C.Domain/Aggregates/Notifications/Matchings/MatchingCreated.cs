namespace Embe.C2C.Domain.Aggregates.Notifications.Matchings;

public class MatchingCreated : MatchingNotification
{
    public MatchingCreated
    (
        Guid recipientUserId,
        Guid matchingId,
        Guid partnerUserId
    ) : base
    (
        recipientUserId,
        matchingId,
        partnerUserId
    )
    {
    }

    private MatchingCreated() : base()
    {

    }
}