using Embe.C2C.Application.Dtos.Aggregates;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;

namespace Embe.C2C.Application.Extensions;

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
