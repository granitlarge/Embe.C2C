namespace Embe.C2C.Application.Dtos.Read.Variants.ValueObjects;

public record DatingPreferencesVariant
{
    public static readonly DatingPreferencesVariant Empty = new
    (
        includeInterestedInGenders: false,
        includeAgeRange: false,
        includeMaximumDistance: false
    );

    public static readonly DatingPreferencesVariant Matched = Empty;

    public static readonly DatingPreferencesVariant Full = new
    (
        includeInterestedInGenders: true,
        includeAgeRange: true,
        includeMaximumDistance: true
    );

    public DatingPreferencesVariant
    (
        bool includeInterestedInGenders,
        bool includeAgeRange,
        bool includeMaximumDistance
    )
    {
        IncludeInterestedInGenders = includeInterestedInGenders;
        IncludeAgeRange = includeAgeRange;
        IncludeMaximumDistance = includeMaximumDistance;
    }

    public bool IncludeInterestedInGenders { get; }
    public bool IncludeAgeRange { get; }
    public bool IncludeMaximumDistance { get; }
}