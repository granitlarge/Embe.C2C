namespace Embe.C2C.Domain.Aggregates.Notifications.Events;

public record NotificationRemovedEvent(Notification Notification) : DomainEvent;