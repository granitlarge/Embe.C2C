using System.Collections.Immutable;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.ValueObjects;

public record DatingPreferencesDto
(
    ImmutableHashSet<Gender> InterestedInGenders,
    int AgeRangeMin,
    int AgeRangeMax,
    DistanceDto MaximumDistance
);

public static class DatingPreferencesDtoExtensions
{
    public static DatingPreferencesDto ToDto(this DatingPreferences datingPreferences)
    {
        return new DatingPreferencesDto
        (
            datingPreferences.InterestedInGenders,
            datingPreferences.AgeRangeMin.Value,
            datingPreferences.AgeRangeMax.Value,
            datingPreferences.MaximumDistance.ToDto()
        );
    }
}