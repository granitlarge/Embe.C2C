using Embe.C2C.Domain.Aggregates.Notifications;

namespace Embe.C2C.Application.Dtos.Aggregates;

public abstract record NotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt
);

public record MatchingCreatedNotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    Guid MatchingId
) : NotificationDto(Id, RecipientUserId, IsRead, ReadAt, CreatedAt);

public record MatchingRemovedNotificationDto
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    Guid MatchingId
) : NotificationDto(Id, RecipientUserId, IsRead, ReadAt, CreatedAt);

public static class NotificationDtoExtensions
{
    public static NotificationDto ToDto(this Notification notification)
    {
        return notification switch
        {
            MatchingCreated mc => new MatchingCreatedNotificationDto(mc.Id, mc.RecipientUserId, mc.IsRead, mc.ReadAt, mc.CreatedAt, mc.MatchingId),
            MatchingRemoved mr => new MatchingRemovedNotificationDto(mr.Id, mr.RecipientUserId, mr.IsRead, mr.ReadAt, mr.CreatedAt, mr.MatchingId),
            _ => throw new ArgumentOutOfRangeException(nameof(notification), $"Unknown notification type: {notification.GetType().Name}")
        };
    }
}
