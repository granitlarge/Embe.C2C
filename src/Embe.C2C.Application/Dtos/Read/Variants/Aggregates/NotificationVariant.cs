namespace Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

public record NotificationVariant
{
    public static readonly NotificationVariant Full = new
    (
        includeId: true,
        includeRecipientUserId: true,
        includeIsRead: true,
        includeReadAt: true,
        includeCreatedAt: true,
        includeUpdatedAt: true,
        includeMatchingId: true,
        includeMessageId: true
    );

    public static readonly NotificationVariant Empty = new
    (
        includeId: false,
        includeRecipientUserId: false,
        includeIsRead: false,
        includeReadAt: false,
        includeCreatedAt: false,
        includeUpdatedAt: false,
        includeMatchingId: false,
        includeMessageId: false
    );

    private NotificationVariant
    (
        bool includeId,
        bool includeRecipientUserId,
        bool includeIsRead,
        bool includeReadAt,
        bool includeCreatedAt,
        bool includeUpdatedAt,

        bool includeMatchingId,

        bool includeMessageId
    )
    {
        IncludeId = includeId;
        IncludeRecipientUserId = includeRecipientUserId;
        IncludeIsRead = includeIsRead;
        IncludeReadAt = includeReadAt;
        IncludeCreatedAt = includeCreatedAt;
        IncludeUpdatedAt = includeUpdatedAt;

        IncludeMatchingId = includeMatchingId;

        IncludeMessageId = includeMessageId;
    }

    public bool IncludeId { get; }
    public bool IncludeRecipientUserId { get; }
    public bool IncludeIsRead { get; }
    public bool IncludeReadAt { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeUpdatedAt { get; }

    public bool IncludeMatchingId { get; }

    public bool IncludeMessageId { get; }
}