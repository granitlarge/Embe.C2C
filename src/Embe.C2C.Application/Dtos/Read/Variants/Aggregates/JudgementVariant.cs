namespace Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

public record JudgementVariant
{

    public static readonly JudgementVariant Empty = new
    (
        includeIsPositive: false,
        includeEditedAt: false,
        includeCreatedAt: false
    );

    public static readonly JudgementVariant Full = new
    (
        includeIsPositive: true,
        includeEditedAt: true,
        includeCreatedAt: true
    );

    public JudgementVariant
    (
        bool includeIsPositive,
        bool includeEditedAt,
        bool includeCreatedAt
    )
    {
        IncludeIsPositive = includeIsPositive;
        IncludeEditedAt = includeEditedAt;
        IncludeCreatedAt = includeCreatedAt;
    }

    public bool IncludeIsPositive { get; }
    public bool IncludeEditedAt { get; }
    public bool IncludeCreatedAt { get; }

}