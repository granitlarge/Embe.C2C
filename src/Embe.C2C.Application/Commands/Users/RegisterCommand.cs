using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.ValueObjects;
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
    string Password,
    DateOnly BirthDate,
    Gender Gender,
    DatingPreferencesDto DatingPreferences,
    ImmutableHashSet<CreateFileDto> Files
);