using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public enum NotificationType
{
    MatchingCreated = 0
}

public abstract record NotificationDto
(
    NotificationType Type,
    Guid? Id,
    Guid? RecipientUserId,
    bool? IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);

public record MatchingCreatedNotificationDto
(
    Guid? Id,
    Guid? RecipientUserId,
    bool? IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? MatchingId
) : NotificationDto(NotificationType.MatchingCreated, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

public class NotificationDtoMapper
{
    public NotificationDto ToDto<T>
    (
        T notification,
        NotificationVariant variant
    ) where T : Notification
    {
        return notification switch
        {
            MatchingCreated matchingCreated => new MatchingCreatedNotificationDto
            (
                variant.IncludeId ? matchingCreated.Id : null,
                variant.IncludeRecipientUserId ? matchingCreated.RecipientUserId : null,
                variant.IncludeIsRead ? matchingCreated.IsRead : null,
                variant.IncludeReadAt ? matchingCreated.ReadAt : null,
                variant.IncludeCreatedAt ? matchingCreated.CreatedAt : null,
                variant.IncludeUpdatedAt ? matchingCreated.UpdatedAt : null,
                variant.IncludeMatchingId ? matchingCreated.MatchingId : null
            ),
            _ => throw new NotImplementedException()
        };

    }
}