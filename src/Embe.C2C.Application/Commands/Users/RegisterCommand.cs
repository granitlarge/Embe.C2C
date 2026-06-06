using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public sealed record RegisterCommand
(
    string Email,
    string Password,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesDto DatingPreferences,
    LocationDto Location,
    ImmutableHashSet<FileDetailsDto> Files
);