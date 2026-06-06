namespace Embe.C2C.Domain.Aggregates.Notifications;

public class MatchingCreated : Notification
{
    public MatchingCreated(Guid recipientUserId, Guid matchingId) : base(recipientUserId)
    {
        MatchingId = matchingId;
    }

    private MatchingCreated()
    {

    }

    public Guid MatchingId { get; }
}