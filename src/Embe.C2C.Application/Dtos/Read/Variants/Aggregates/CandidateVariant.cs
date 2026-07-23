namespace Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

public record CandidateVariant
{
    public static readonly CandidateVariant Empty = new
    (
        includeJudgement: false,
        includeCreatedAt: false,
        includeUpdatedAt: false
    );

    public static readonly CandidateVariant Full = new
    (
        includeJudgement: true,
        includeCreatedAt: true,
        includeUpdatedAt: true
    );

    public CandidateVariant
    (
        bool includeJudgement,
        bool includeCreatedAt,
        bool includeUpdatedAt
    )
    {
        IncludeJudgement = includeJudgement;
        IncludeCreatedAt = includeCreatedAt;
        IncludeUpdatedAt = includeUpdatedAt;
    }

    public bool IncludeJudgement { get; }
    public bool IncludeCreatedAt { get; }
    public bool IncludeUpdatedAt { get; }
}