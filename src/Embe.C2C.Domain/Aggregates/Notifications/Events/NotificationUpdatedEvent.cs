namespace Embe.C2C.Domain.Aggregates.Notifications.Events;

public record NotificationUpdatedEvent(Notification Notification) : DomainEvent;