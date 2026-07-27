namespace Embe.C2C.Domain.Aggregates.Notifications.Candidates;

public class PositivelyJudgedNotification : Notification
{
    private PositivelyJudgedNotification(Guid candidateId, Guid recipientUserId) : base(recipientUserId)
    {
        CandidateId = candidateId;
    }

    public Guid CandidateId { get; private set; }

    public static PositivelyJudgedNotification Create(Guid CandidateId, Guid recipientUserId)
    {
        return new PositivelyJudgedNotification(CandidateId, recipientUserId);
    }

}