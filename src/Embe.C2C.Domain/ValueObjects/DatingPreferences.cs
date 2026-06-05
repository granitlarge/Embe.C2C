using System.Collections.Immutable;
using Embe.C2C.Domain.Exceptions;

namespace Embe.C2C.Domain.ValueObjects;

public record DatingPreferences
{
    public DatingPreferences
    (
        ImmutableHashSet<Gender> interestedInGenders,
        Age ageRangeMin,
        Age ageRangeMax,
        Distance maximumDistance
    )
    {
        if (interestedInGenders.Count == 0)
        {
            throw new DomainException("At least one interested in gender must be specified.");
        }

        if (ageRangeMin > ageRangeMax)
        {
            throw new DomainException("Minimum age cannot be greater than maximum age.");
        }

        if (ageRangeMin < new Age(18))
        {
            throw new DomainException("Minimum age must be at least 18.");
        }

        InterestedInGenders = [.. interestedInGenders];
        AgeRangeMin = ageRangeMin;
        AgeRangeMax = ageRangeMax;
        MaximumDistance = maximumDistance;
    }

    public ImmutableHashSet<Gender> InterestedInGenders { get; }
    public Age AgeRangeMin { get; }
    public Age AgeRangeMax { get; }
    public Distance MaximumDistance { get; }
}