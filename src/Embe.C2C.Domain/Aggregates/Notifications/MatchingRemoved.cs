namespace Embe.C2C.Domain.Aggregates.Notifications;

public class MatchingRemoved(Guid RecipientUserId, Guid MatchingId) : Notification(RecipientUserId)
{
    public Guid MatchingId { get; } = MatchingId;
}