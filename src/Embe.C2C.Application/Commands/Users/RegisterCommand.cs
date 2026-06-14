using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Write.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users;

public record CreateFileDto
(
    string Url,
    string MimeType,
    int Order
);

public sealed record RegisterCommand
(
    string Email,
    string UserName,
    string Password,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesWriteDto DatingPreferences,
    ImmutableHashSet<CreateFileDto> Files
);