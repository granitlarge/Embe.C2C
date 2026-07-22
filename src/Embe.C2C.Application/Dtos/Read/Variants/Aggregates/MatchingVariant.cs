namespace Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

public record MatchingVariant
{
    public static readonly MatchingVariant Empty = new
    (
        includeCreatedAt: false
    );

    public static readonly MatchingVariant Full = new
    (
        includeCreatedAt: true
    );

    public MatchingVariant
    (
        bool includeCreatedAt
    )
    {
        IncludeCreatedAt = includeCreatedAt;
    }

    public bool IncludeCreatedAt { get; }
}

public record MessageVariant
{
    public static readonly MessageVariant Empty = new
    (
        includeContent: false,
        includeSeenAt: false,
        includeCreatedAt: false,
        includeEditedAt: false,
        includeIsReply: false,
        includeReplyToMessage: false
    );

    public static readonly MessageVariant Full = new
    (
        includeContent: true,
        includeSeenAt: true,
        includeCreatedAt: true,
        includeEditedAt: true,
        includeIsReply: true,
        includeReplyToMessage: true
    );

    public MessageVariant
    (
        bool includeContent,
        bool includeSeenAt,
        bool includeCreatedAt,
        bool includeEditedAt,
        bool includeIsReply,
        bool includeReplyToMessage
    )
    {
        IncludeContent = includeContent;
        IncludeSeenAt = includeSeenAt;
        IncludeCreatedAt = includeCreatedAt;
        IncludeEditedAt = includeEditedAt;
        IncludeIsReply = includeIsReply;
        IncludeReplyToMessage = includeReplyToMessage;
    }

    public bool IncludeContent { get; }
    public bool IncludeSeenAt { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeEditedAt { get; }
    public bool IncludeIsReply { get; }
    public bool IncludeReplyToMessage { get; }
}