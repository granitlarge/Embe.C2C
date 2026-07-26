namespace Embe.C2C.Application.Events.Notifications;

public record NotificationCreatedIntegrationEvent(Guid RecipientUserId, Guid NotificationId) : IntegrationEvent(IntegrationEventType.NotificationCreated);