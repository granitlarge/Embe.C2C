namespace Embe.C2C.Application.Dtos.Read.Variants.Aggregates;

public record SearchProfileVariant
{
    public static readonly SearchProfileVariant Empty = new
    (
        includeName: false,
        includeDescription: false,
        includeRelationshipType: false,
        includeEngagement: false,
        includeGenders: false,
        includeAgeRange: false,
        includeMaximumDistance: false
    );

    public static readonly SearchProfileVariant Candidate = new
    (
        includeName: false,
        includeDescription: true,
        includeRelationshipType: true,
        includeEngagement: true,
        includeGenders: false,
        includeAgeRange: false,
        includeMaximumDistance: false
    );

    public static readonly SearchProfileVariant Matched = new
    (
        includeName: false,
        includeDescription: true,
        includeRelationshipType: true,
        includeEngagement: true,
        includeGenders: false,
        includeAgeRange: false,
        includeMaximumDistance: false
    );

    public static readonly SearchProfileVariant Full = new
    (
        includeName: true,
        includeDescription: true,
        includeRelationshipType: true,
        includeEngagement: true,
        includeGenders: true,
        includeAgeRange: true,
        includeMaximumDistance: true
    );

    private SearchProfileVariant
    (
        bool includeName,
        bool includeDescription,
        bool includeRelationshipType,
        bool includeEngagement,
        bool includeGenders,
        bool includeAgeRange,
        bool includeMaximumDistance
    )
    {
        IncludeName = includeName;
        IncludeDescription = includeDescription;
        IncludeRelationshipType = includeRelationshipType;
        IncludeEngagement = includeEngagement;
        IncludeGenders = includeGenders;
        IncludeAgeRange = includeAgeRange;
        IncludeMaximumDistance = includeMaximumDistance;
    }

    public bool IncludeName { get; }
    public bool IncludeDescription { get; }
    public bool IncludeRelationshipType { get; }
    public bool IncludeEngagement { get; }
    public bool IncludeGenders { get; }
    public bool IncludeAgeRange { get; }
    public bool IncludeMaximumDistance { get; }
}