using System.Collections.Immutable;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public record UpdateCommand
(
    Guid UserId,
    string Email,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesDto DatingPreferences,
    Location Location,
    ImmutableHashSet<Guid> FilesToKeep,
    ImmutableHashSet<FileDetailsDto> FilesToAdd
);