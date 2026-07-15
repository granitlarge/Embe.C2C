using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Events.Images;

public record ImageRemovedEvent(Guid UserId, Guid ImageId, string ImageName, ImageStatus ImageStatus) : IntegrationEvent(IntegrationEventType.ImageRemoved);
