namespace Embe.C2C.Application.Events.Images;

public record ImageRemovedEvent(string Url) : IntegrationEvent(IntegrationEventType.ImageRemoved);
public record ImageMovedEvent(string FromUrl, string ToUrl) : IntegrationEvent(IntegrationEventType.ImageMoved);