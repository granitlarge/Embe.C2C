namespace Embe.C2C.Application.Events;

public enum IntegrationEventType
{
    NotificationCreated,
    NotificationRemoved,

    MessageCreated,
    MessageEdited,
    MessageDeleted,
    MessageSeen,
    MessageUnseen,

    ImageRemoved,
    ImageMoved,
    ImageResized,

    MatchingRemoved,
    MatchingCreated,

    PositivelyJudged,
    SearchProfileUpdated,
}

public record IntegrationEvent(IntegrationEventType Type)
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}