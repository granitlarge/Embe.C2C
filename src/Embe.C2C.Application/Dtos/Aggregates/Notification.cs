using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Dtos.Aggregates;

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

public record MatchingCreatedNotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid MatchingId
) : NotificationDto(NotificationType.MatchingCreated, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

public record MatchingRemovedNotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid MatchingId
) : NotificationDto(NotificationType.MatchingRemoved, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

public static class NotificationDtoExtensions
{
    public static NotificationDto ToDto(this Notification notification)
    {
        return notification switch
        {
            MatchingCreated mc => new MatchingCreatedNotificationDto(mc.Id, mc.RecipientUserId, mc.IsRead, mc.ReadAt, mc.CreatedAt, mc.UpdatedAt, mc.MatchingId),
            MatchingRemoved mr => new MatchingRemovedNotificationDto(mr.Id, mr.RecipientUserId, mr.IsRead, mr.ReadAt, mr.CreatedAt, mr.UpdatedAt, mr.MatchingId),
            _ => throw new ArgumentOutOfRangeException(nameof(notification), $"Unknown notification type: {notification.GetType().Name}")
        };
    }
}
