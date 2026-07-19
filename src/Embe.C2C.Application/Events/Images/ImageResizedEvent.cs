namespace Embe.C2C.Application.Events.Images;

public record ImageResizedEvent
(
    Guid UserId,
    Guid ImageId,
    string? OriginalUrl,
    string? LargeUrl,
    string? MediumUrl,
    string? SmallUrl
) : IntegrationEvent(IntegrationEventType.ImageResized);