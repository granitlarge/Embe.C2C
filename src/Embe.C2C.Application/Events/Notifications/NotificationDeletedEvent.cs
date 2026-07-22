namespace Embe.C2C.Application.Events.Notifications;

public record NotificationDeletedEvent(Guid UserId, Guid NotificationId) : IntegrationEvent(IntegrationEventType.NotificationDeleted);