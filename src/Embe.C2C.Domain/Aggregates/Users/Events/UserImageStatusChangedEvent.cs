using Embe.C2C.Domain.Entities;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Users.Events;

public record UserImageStatusChangedEvent
(
    ImageStatus OldStatus,
    Image Image
) : DomainEvent();