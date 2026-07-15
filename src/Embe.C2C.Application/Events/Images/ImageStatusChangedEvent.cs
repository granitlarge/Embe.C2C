using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Events.Images;

public record ImageStatusChangedEvent(Guid UserId, Guid ImageId, string ImageName, ImageStatus OldStatus, ImageStatus NewStatus) : IntegrationEvent(IntegrationEventType.ImageMoved);