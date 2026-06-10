namespace Embe.C2C.Domain.Aggregates.Judgements.Events;

public record LikeEvent(Guid LikerUserId, Guid LikeeUserId) : DomainEvent;