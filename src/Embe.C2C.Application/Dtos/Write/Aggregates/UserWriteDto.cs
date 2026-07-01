using System.Collections.Immutable;
using Embe.C2C.Application.Dtos.Write.Entities;
using Embe.C2C.Application.Dtos.Write.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Write.Aggregates;

public record UserWriteDto
(
    Guid Id,
    string? Email,
    string? UserName,
    DateOnly? BirthDate,
    Gender? Gender,
    LocationWriteDto? Location,
    FileWriteDto? ProfilePicture,
    ImmutableHashSet<FileWriteDto>? Files,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);