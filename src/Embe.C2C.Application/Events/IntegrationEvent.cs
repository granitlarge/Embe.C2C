namespace Embe.C2C.Application.Events;

public enum IntegrationEventType
{
    NotificationCreated,

    MessageCreated,
    MessageEdited,
    MessageDeleted,
    MessageSeen,
    MessageUnseen,

    ImageRemoved,
    ImageMoved,
    ImageResized,

    MatchingRemoved,
    MatchingCreated
}

public record IntegrationEvent(IntegrationEventType Type)
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}