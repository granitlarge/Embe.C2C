using System.Collections.Immutable;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Write.ValueObjects;

public record DatingPreferencesWriteDto
(
    ImmutableHashSet<Gender> InterestedInGenders,
    int AgeRangeMin,
    int AgeRangeMax,
    DistanceWriteDto MaximumDistance
);