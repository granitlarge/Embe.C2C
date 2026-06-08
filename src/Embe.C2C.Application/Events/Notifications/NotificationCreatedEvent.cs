using Embe.C2C.Application.Dtos.Aggregates;

namespace Embe.C2C.Application.Events.Notifications;

public record NotificationCreatedEvent(NotificationDto Notification) : IntegrationEvent(IntegrationEventType.NotificationCreated);
public record NotificationUpdatedEvent(NotificationDto Notification) : IntegrationEvent(IntegrationEventType.NotificationUpdated);
public record NotificationDeletedEvent(Guid NotificationId) : IntegrationEvent(IntegrationEventType.NotificationDeleted);