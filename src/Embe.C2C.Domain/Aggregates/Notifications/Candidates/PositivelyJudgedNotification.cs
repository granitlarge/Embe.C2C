namespace Embe.C2C.Domain.Aggregates.Notifications.Candidates;

public class PositivelyJudgedNotification : Notification
{
    private PositivelyJudgedNotification
    (
        Guid candidateId,
        Guid recipientUserId,
        Guid userId,
        Guid candidateUserId
    ) : base(recipientUserId)
    {
        CandidateId = candidateId;
        UserId = userId;
        CandidateUserId = candidateUserId;
    }

    public Guid CandidateId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CandidateUserId { get; private set; }

    public static PositivelyJudgedNotification Create
    (
        Guid CandidateId, 
        Guid recipientUserId,
        Guid userId,
        Guid candidateUserId
    )
    {
        return new PositivelyJudgedNotification(CandidateId, recipientUserId, userId, candidateUserId);
    }

}