namespace Embe.C2C.Application.Events.Notifications;

public abstract record NotificationIntegrationEvent(Guid RecipientUserId, Guid NotificationId, IntegrationEventType Type) : IntegrationEvent(Type);