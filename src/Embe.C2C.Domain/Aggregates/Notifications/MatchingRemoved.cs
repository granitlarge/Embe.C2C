namespace Embe.C2C.Domain.Aggregates.Notifications;

public class MatchingRemoved : Notification
{
    public MatchingRemoved(Guid recipientUserId, Guid matchingId) : base(recipientUserId)
    {
        MatchingId = matchingId;
    }

    private MatchingRemoved()
    {

    }

    public Guid MatchingId { get; }
}