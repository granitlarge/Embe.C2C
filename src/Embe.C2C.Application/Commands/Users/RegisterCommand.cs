using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Write.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public record CreateImageDto
(
    string Url,
    string MimeType,
    int Order
);

public sealed record RegisterCommand
(
    string Email,
    string Password,
    string Alias,
    Gender Gender,
    LocationWriteDto Location,
    DateOnly BirthDate
);

public sealed record CreateSearchProfileDto
(
    string Name,
    string Description,
    RelationshipType RelationshipType,
    EngagementWriteDto Engagement,
    ImmutableHashSet<Gender> Genders,
    Age? AgeRangeMin,
    Age? AgeRangeMax,
    Distance? MaximumDistance
);