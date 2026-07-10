using System.Collections.Immutable;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.SearchProfiles;

public record UpdateSearchProfileCommand
(
    Guid Id,
    string Name,
    string Description,
    RelationshipType RelationshipType,
    EngagementWriteDto Engagement,
    ImmutableHashSet<Gender> Genders,
    int? AgeRangeMin,
    int? AgeRangeMax,
    int? MaximumDistanceKm
);