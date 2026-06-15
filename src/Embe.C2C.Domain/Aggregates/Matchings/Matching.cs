using Embe.C2C.Domain.Aggregates.Matchings.Events;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Entities;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.Aggregates.Matchings;

public class Matching : Aggregate
{
    private Matching
    (
        Guid userId1,
        Guid userId2
    )
    {
        if (userId1 == userId2)
        {
            throw new DomainException(new DomainError<MatchingError>(MatchingError.SelfMatching));
        }

        Id = Guid.CreateVersion7();
        UserId1 = userId1;
        UserId2 = userId2;
        Conversation = Conversation.Create(Id, userId1, userId2);
        CreatedAt = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Matching() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Guid Id { get; }
    public Guid UserId1 { get; }
    public Guid UserId2 { get; }
    public Conversation Conversation { get; }
    public DateTimeOffset CreatedAt { get; private set; }

    public void Remove(Guid actorUserId)
    {
        if (actorUserId != UserId1 && actorUserId != UserId2)
        {
            throw new DomainException(new DomainError<MatchingError>(MatchingError.Unauthorized));
        }
        AddDomainEvent(new MatchingRemovedEvent(actorUserId, this));
    }

    internal static Matching Create(Guid userId1, Guid userId2)
    {
        return new Matching(userId1, userId2);
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
    #endregion
}

public enum MatchingError
{
    SelfMatching,
    Unauthorized
}