using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Read.ValueObjects;
using Embe.C2C.Application.Dtos.Write.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public sealed record RegisterCommand
(
    string Email,
    string EmailVerificationCode,
    string Password,
    string Alias,
    Gender Gender,
    DateOnly BirthDate,
    ImageWriteDto[] Images,
    LocationDto? Location
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