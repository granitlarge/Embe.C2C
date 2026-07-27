namespace Embe.C2C.Application.Events.Notifications;

public record NotificationCreatedIntegrationEvent(Guid RecipientUserId, Guid NotificationId) : NotificationIntegrationEvent(RecipientUserId, NotificationId, IntegrationEventType.NotificationCreated);