using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Read.Variants.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record DatingPreferencesDto
(
    ImmutableHashSet<Gender>? InterestedInGenders,
    int? AgeRangeMin,
    int? AgeRangeMax,
    DistanceDto? MaximumDistance
);

public static class DatingPreferencesDtoExtensions
{
    public static DatingPreferencesDto? ToDto(this DatingPreferences datingPreferences, DatingPreferencesVariant variant)
    {
        if (variant == DatingPreferencesVariant.Empty)
        {
            return null;
        }

        return new DatingPreferencesDto
        (
            variant.IncludeInterestedInGenders ? datingPreferences.InterestedInGenders : null,
            variant.IncludeAgeRange ? datingPreferences.AgeRangeMin.Value : null,
            variant.IncludeAgeRange ? datingPreferences.AgeRangeMax.Value : null,
            variant.IncludeMaximumDistance ? datingPreferences.MaximumDistance.ToDto() : null
        );
    }
}