using Embe.C2C.Application.Dtos.Read.Aggregates;

namespace Embe.C2C.Application.Events.Notifications;

public record NotificationUpdatedEvent(NotificationDto Notification) : IntegrationEvent(IntegrationEventType.NotificationUpdated);
