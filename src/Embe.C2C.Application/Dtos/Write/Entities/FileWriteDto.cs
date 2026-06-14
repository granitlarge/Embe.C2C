using Embe.C2C.Application.Dtos.Write.ValueObjects;

namespace Embe.C2C.Application.Dtos.Write.Entities;

public record FileWriteDto
(
    Guid Id,
    Guid OwnerUserId,
    FileDetailsWriteDto? FileDetails,
    DateTimeOffset? MarkedForDeletionAt,
    DateTimeOffset? DeletedAt,
    DateTimeOffset? CreatedAt
);