namespace Embe.C2C.Domain.Aggregates.Notifications;

public class MatchingCreated
(
    Guid recipientUserId,
    Guid matchingId
) : Notification(recipientUserId)
{
    public Guid MatchingId { get; } = matchingId;
}