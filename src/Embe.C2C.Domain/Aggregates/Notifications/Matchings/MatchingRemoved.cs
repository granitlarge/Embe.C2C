namespace Embe.C2C.Domain.Aggregates.Notifications.Matchings;

public class MatchingRemoved : MatchingNotification
{
    public MatchingRemoved(
        Guid recipientUserId,
        Guid matchingId,
        Guid partnerUserId,
        string partnerUserName,
        string partnerProfileImageUrl
    ) : base(
        recipientUserId,
        matchingId,
        partnerUserId,
        partnerUserName,
        partnerProfileImageUrl
    )
    {
        
    }

    private MatchingRemoved()
    {

    }
}