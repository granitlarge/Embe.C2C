using System.Collections.Immutable;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public sealed record RegisterCommand
(
    string Email,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesDto DatingPreferences,
    Location Location,
    ImmutableHashSet<FileDetailsDto> Files
);

public record DatingPreferencesDto
(
    Gender[] InterestedInGenders,
    int AgeRangeMin,
    int AgeRangeMax,
    Distance MaximumDistance
);

public record FileDetailsDto
(
    string MimeType,
    byte[] Content
);