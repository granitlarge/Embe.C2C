namespace Embe.C2C.Domain.Aggregates.Notifications;

public class MatchingRemoved : Notification
{
    public MatchingRemoved(Guid recipientUserId, Guid matchingId, Guid partnerUserId) : base(recipientUserId)
    {
        MatchingId = matchingId;
        PartnerUserId = partnerUserId;
    }

    private MatchingRemoved()
    {

    }

    public Guid MatchingId { get; }
    public Guid PartnerUserId { get; }
}