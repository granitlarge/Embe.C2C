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
            throw new DomainException(new DomainError<DatingPreferencesError>(DatingPreferencesError.InvalidInterestedInGenders));
        }

        if (ageRangeMin > ageRangeMax)
        {
            throw new DomainException(new DomainError<DatingPreferencesError>(DatingPreferencesError.InvalidAgeRange));
        }

        if (ageRangeMin < new Age(18))
        {
            throw new DomainException(new DomainError<DatingPreferencesError>(DatingPreferencesError.InvalidAgeRange));
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

public enum DatingPreferencesError
{
    InvalidInterestedInGenders,
    InvalidAgeRange
}