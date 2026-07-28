using System.Text.Json.Serialization;
using Embe.C2C.Application.Dtos.Read.Variants.Aggregates;
using Embe.C2C.Domain.Aggregates.Notifications;
using Embe.C2C.Domain.Aggregates.Notifications.Candidates;
using Embe.C2C.Domain.Aggregates.Notifications.Matchings;
using Embe.C2C.Domain.Aggregates.Notifications.Messages;

namespace Embe.C2C.Application.Dtos.Read.Aggregates;

public enum NotificationType
{
    MatchingCreated = 0,
    MessageCreated = 1,
    PositivelyJudged = 2,
}

[JsonPolymorphic]
[JsonDerivedType(typeof(MatchingCreatedNotificationDto))]
[JsonDerivedType(typeof(MessageCreatedNotificationDto))]
[JsonDerivedType(typeof(PositivelyJudgedNotificationDto))]
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
    Guid? MatchingId,
    Guid? UserId1,
    Guid? UserId2
) : NotificationDto(NotificationType.MatchingCreated, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

public record MessageCreatedNotificationDto
(
    Guid? Id,
    Guid? RecipientUserId,
    bool? IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? MessageId
) : NotificationDto(NotificationType.MessageCreated, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

public record PositivelyJudgedNotificationDto
(
    Guid? Id,
    Guid? RecipientUserId,
    bool? IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? CandidateId,
    Guid? UserId,
    Guid? CandidateUserId
) : NotificationDto(NotificationType.PositivelyJudged, Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt);

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
            MatchingCreatedNotification matchingCreated => new MatchingCreatedNotificationDto
            (
                variant.IncludeId ? matchingCreated.Id : null,
                variant.IncludeRecipientUserId ? matchingCreated.RecipientUserId : null,
                variant.IncludeIsRead ? matchingCreated.IsRead : null,
                variant.IncludeReadAt ? matchingCreated.ReadAt : null,
                variant.IncludeCreatedAt ? matchingCreated.CreatedAt : null,
                variant.IncludeUpdatedAt ? matchingCreated.UpdatedAt : null,
                variant.IncludeMatchingId ? matchingCreated.MatchingId : null,
                variant.IncludeUserId1 ? matchingCreated.RecipientUserId : null,
                variant.IncludeUserId2 ? matchingCreated.PartnerUserId : null
            ),
            MessageCreatedNotification messageCreated => new MessageCreatedNotificationDto
            (
                variant.IncludeId ? messageCreated.Id : null,
                variant.IncludeRecipientUserId ? messageCreated.RecipientUserId : null,
                variant.IncludeIsRead ? messageCreated.IsRead : null,
                variant.IncludeReadAt ? messageCreated.ReadAt : null,
                variant.IncludeCreatedAt ? messageCreated.CreatedAt : null,
                variant.IncludeUpdatedAt ? messageCreated.UpdatedAt : null,
                variant.IncludeMessageId ? messageCreated.MessageId : null
            ),
            PositivelyJudgedNotification positivelyJudged => new PositivelyJudgedNotificationDto
            (
                variant.IncludeId ? positivelyJudged.Id : null,
                variant.IncludeRecipientUserId ? positivelyJudged.RecipientUserId : null,
                variant.IncludeIsRead ? positivelyJudged.IsRead : null,
                variant.IncludeReadAt ? positivelyJudged.ReadAt : null,
                variant.IncludeCreatedAt ? positivelyJudged.CreatedAt : null,
                variant.IncludeUpdatedAt ? positivelyJudged.UpdatedAt : null,
                variant.IncludeCandidateId ? positivelyJudged.CandidateId : null,
                variant.IncludeUserId ? positivelyJudged.UserId : null,
                variant.IncludeCandidateUserId ? positivelyJudged.CandidateUserId : null
            ),
            _ => throw new NotImplementedException()
        };

    }
}