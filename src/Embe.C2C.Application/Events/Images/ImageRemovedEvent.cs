namespace Embe.C2C.Application.Events.Images;

public record ImageRemovedEvent(Guid UserId, Guid ImageId, string ImageName) : IntegrationEvent(IntegrationEventType.ImageRemoved);