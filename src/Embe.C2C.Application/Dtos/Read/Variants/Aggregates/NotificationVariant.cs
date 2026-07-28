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
        includeMessageId: true,
        includeCandidateId: true,
        includeCandidateUserId: true,
        includeUserId: true,
        includeUserId1: true,
        includeUserId2: true
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
        includeMessageId: false,
        includeCandidateId: false,
        includeCandidateUserId: false,
        includeUserId: false,
        includeUserId1: false,
        includeUserId2: false
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
        bool includeUserId1,
        bool includeUserId2,

        bool includeMessageId,

        bool includeCandidateId,
        bool includeUserId,
        bool includeCandidateUserId
    )
    {
        IncludeId = includeId;
        IncludeRecipientUserId = includeRecipientUserId;
        IncludeIsRead = includeIsRead;
        IncludeReadAt = includeReadAt;
        IncludeCreatedAt = includeCreatedAt;
        IncludeUpdatedAt = includeUpdatedAt;

        IncludeMatchingId = includeMatchingId;
        IncludeUserId1 = includeUserId1;
        IncludeUserId2 = includeUserId2;

        IncludeMessageId = includeMessageId;

        IncludeCandidateId = includeCandidateId;
        IncludeUserId = includeUserId;
        IncludeCandidateUserId = includeCandidateUserId;
    }

    public bool IncludeId { get; }
    public bool IncludeRecipientUserId { get; }
    public bool IncludeIsRead { get; }
    public bool IncludeReadAt { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeUpdatedAt { get; }

    public bool IncludeMatchingId { get; }
    public bool IncludeUserId1 { get; }
    public bool IncludeUserId2 { get; }

    public bool IncludeMessageId { get; }

    public bool IncludeCandidateId { get; }
    public bool IncludeUserId { get; }
    public bool IncludeCandidateUserId { get; }
}