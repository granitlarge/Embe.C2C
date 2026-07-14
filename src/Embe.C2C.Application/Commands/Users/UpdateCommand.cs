using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Write.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public record UpdateCommand
(
    Guid UserId,
    string Alias,
    DateOnly BirthDate,
    Gender? Gender,
    LocationWriteDto? Location,
    ImmutableHashSet<UpdateImageDto>? ImagesToKeep,
    string? Bio
);

public record UpdateImageDto
(
    Guid Id,
    int Order
);