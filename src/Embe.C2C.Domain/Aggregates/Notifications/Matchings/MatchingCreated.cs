
namespace Embe.C2C.Domain.Aggregates.Notifications.Matchings;

public class MatchingCreatedNotification : MatchingNotification
{
    public MatchingCreatedNotification
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

    private MatchingCreatedNotification() : base()
    {

    }

    public static MatchingCreatedNotification Create
    (
        Guid recipientUserId,
        Guid matchingId,
        Guid partnerUserId
    )
    {
        return new MatchingCreatedNotification(recipientUserId, matchingId, partnerUserId);
    }
}