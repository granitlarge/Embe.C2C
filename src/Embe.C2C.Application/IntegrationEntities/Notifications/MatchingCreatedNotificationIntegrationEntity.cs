using Embe.C2C.Application.Dtos.Aggregates;

namespace Embe.C2C.Application.IntegrationEntities.Notifications;

public record MatchingCreatedNotificationIntegrationEntity
(
    Guid Id,
    Guid RecipientUserId,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid MatchingId,
    string PartnerUserName,
    string PartnerProfileImageUrl
) : MatchingCreatedNotificationDto(Id, RecipientUserId, IsRead, ReadAt, CreatedAt, UpdatedAt, MatchingId);