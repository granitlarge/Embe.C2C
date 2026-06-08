namespace Embe.C2C.Domain.Aggregates.Notifications;

public class MatchingCreated : Notification
{
    public MatchingCreated(Guid recipientUserId, Guid matchingId, Guid partnerUserId) : base(recipientUserId)
    {
        MatchingId = matchingId;
        PartnerUserId = partnerUserId;
    }

    private MatchingCreated()
    {

    }

    public Guid MatchingId { get; }
    public Guid PartnerUserId { get; }
}