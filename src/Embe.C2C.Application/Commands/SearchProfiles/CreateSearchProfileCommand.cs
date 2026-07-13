using System.Collections.Immutable;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements.Enums;

namespace Embe.C2C.Application.Commands.SearchProfiles;

public record CreateSearchProfileCommand
(
    string Name,
    string Description,
    RelationshipType RelationshipType,
    EngagementWriteDto Engagement,
    ImmutableHashSet<Gender> Genders,
    int? AgeRangeMin,
    int? AgeRangeMax,
    double? MaximumDistanceKm
);

public record EngagementWriteDto
(
    EngagementMedium Medium,
    EngagementBoundedness Boundedness,
    EngagementFrequency Frequency,
    DateOnly? StartDate,
    DateOnly? EndDate
);