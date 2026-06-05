using Embe.C2C.Domain.Aggregates.Matchings.Events;
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
            throw new DomainException("A user cannot be matched with themselves.");
        }

        Id = Guid.CreateVersion7();
        UserId1 = userId1;
        UserId2 = userId2;
        Conversation = Conversation.Create(Id, userId1, userId2);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }
    public Guid UserId1 { get; }
    public Guid UserId2 { get; }
    public Conversation Conversation { get; }
    public DateTimeOffset CreatedAt { get; }

    public void Remove(Guid actorUserId)
    {
        if (actorUserId != UserId1 && actorUserId != UserId2)
        {
            throw new DomainException("Only users involved in the matching can remove it.");
        }
        AddDomainEvent(new MatchingRemovedEvent(actorUserId, this));
    }

    internal static Matching Create(Guid userId1, Guid userId2)
    {
        return new Matching(userId1, userId2);
    }
}