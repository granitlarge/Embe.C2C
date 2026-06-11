using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public record UpdateCommand
(
    Guid UserId,
    string Email,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesDto DatingPreferences,
    LocationDto? Location,
    ImmutableHashSet<Guid> FilesToKeep,
    ImmutableHashSet<CreateFileDto> FilesToAdd
);