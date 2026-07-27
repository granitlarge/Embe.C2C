namespace Embe.C2C.Application.Events.Notifications;

public record NotificationRemovedIntegrationEvent(Guid NotificationId, Guid RecipientUserId) : IntegrationEvent(IntegrationEventType.NotificationRemoved);