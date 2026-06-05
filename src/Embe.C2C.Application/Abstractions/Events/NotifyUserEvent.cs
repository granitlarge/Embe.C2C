using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Abstractions.Events;

public record NotificationCreatedEvent(Notification Notification) : ApplicationEvent;