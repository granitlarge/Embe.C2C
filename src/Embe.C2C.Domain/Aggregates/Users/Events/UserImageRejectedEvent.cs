using Embe.C2C.Domain.Entities;

namespace Embe.C2C.Domain.Aggregates.Users.Events;

public record UserImageRejectedEvent(Image Image) : DomainEvent();