namespace Embe.C2C.Domain.Aggregates.Notifications.Events;

public record NotificationCreatedEvent<T>(T Notitifiation) where T : Notification;