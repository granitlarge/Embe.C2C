namespace Embe.C2C.Domain.Aggregates.Notifications.Matchings;

public class MatchingCreated : MatchingNotification
{
    public MatchingCreated
    (
        Guid recipientUserId,
        Guid matchingId,
        Guid partnerUserId,
        string partnerUserName,
        string? partnerProfileImageUrl
    ) : base
    (
        recipientUserId,
        matchingId,
        partnerUserId,
        partnerUserName,
        partnerProfileImageUrl
    )
    {
    }

    private MatchingCreated() : base()
    {

    }
}