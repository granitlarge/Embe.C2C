namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public enum NotificationType
{
    MatchingCreated = 0,
    MatchingRemoved = 1
}

public abstract record NotificationDto
(
    NotificationType Type,
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public abstract record MatchingNotificationDto
(
    NotificationType Type,
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid MatchingId
) : NotificationDto(Type, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

public record MatchingCreatedNotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid MatchingId
) : MatchingNotificationDto(NotificationType.MatchingCreated, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt, MatchingId);

public record MatchingRemovedNotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid MatchingId
) : MatchingNotificationDto(NotificationType.MatchingRemoved, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt, MatchingId);
