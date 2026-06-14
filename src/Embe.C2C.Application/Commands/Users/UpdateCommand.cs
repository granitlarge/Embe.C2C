using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Write.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public record UpdateCommand
(
    Guid UserId,
    string Email,
    string UserName,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesWriteDto DatingPreferences,
    LocationWriteDto? Location,
    ImmutableHashSet<Guid> FilesToKeep,
    ImmutableHashSet<CreateFileDto> FilesToAdd
);