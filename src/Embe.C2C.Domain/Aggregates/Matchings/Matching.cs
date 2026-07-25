using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Aggregates.Users;
using ErrorOr;

namespace Embe.C2C.Domain.Aggregates.Matchings;

public class Matching : Aggregate
{
    private Matching
    (
        Guid userId1,
        Guid userId2,
        Guid userId1SearchProfileId,
        Guid userId2SearchProfileId
    )
    {
        Id = Guid.CreateVersion7();
        UserId1 = userId1;
        UserId2 = userId2;
        UserId1SearchProfileId = userId1SearchProfileId;
        UserId2SearchProfileId = userId2SearchProfileId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Matching() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Guid Id { get; private set; }
    public Guid UserId1 { get; private set; }
    public Guid UserId2 { get; private set; }
    public Guid? UserId1SearchProfileId { get; private set; }
    public Guid? UserId2SearchProfileId { get; private set; }
    public Guid? LastMessageId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public void UpdateLastMessageId(Guid? lastMessageId)
    {
        LastMessageId = lastMessageId;
    }

    public ErrorOr<Success> Remove(Guid actorUserId)
    {
        if (actorUserId != UserId1 && actorUserId != UserId2)
        {
            return DomainErrors.Forbidden.ToForbiddenErrorOr();
        }
        AddDomainEvent(new MatchingRemovedEvent(actorUserId, this));
        return Result.Success;
    }

    internal static ErrorOr<Matching> Create
    (
        Guid userId1,
        Guid userId2,
        Guid userId1SearchProfileId,
        Guid userId2SearchProfileId
    )
    {
        if (userId1 == userId2)
        {
            return DomainErrors.UserSame.ToValidationErrorOr();
        }

        return new Matching(userId1, userId2, userId1SearchProfileId, userId2SearchProfileId);
    }

    public Guid? GetOtherUserId(Guid? userId)
    {
        if (userId == UserId1) return UserId2;
        if (userId == UserId2) return UserId1;
        return null;
    }

    #region Read Only Navigation Properties
    public User? User1 { get; private set; }
    public User? User2 { get; private set; }
    public SearchProfile? User1SearchProfile { get; private set; }
    public SearchProfile? User2SearchProfile { get; private set; }
    public List<Messages.Message>? Messages { get; private set; }
    public Messages.Message? LastMessage { get; private set; }
    #endregion
}
