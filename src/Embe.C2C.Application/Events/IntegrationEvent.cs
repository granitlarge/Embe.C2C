namespace Embe.C2C.Application.Events;

public enum IntegrationEventType
{
    NotificationCreated,
    NotificationUpdated,
    NotificationDeleted
}

public record IntegrationEvent(IntegrationEventType Type)
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}