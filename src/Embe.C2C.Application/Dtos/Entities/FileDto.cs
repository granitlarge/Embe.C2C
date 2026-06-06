using Embe.C2C.Application.Dtos.ValueObjects;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Entities;

public record FileDto
(
    Guid Id,
    Guid OwnerUserId,
    FileDetailsDto FileDetails,
    DateTimeOffset? MarkedForDeletionAt,
    DateTimeOffset? DeletedAt,
    DateTimeOffset CreatedAt
);

public static class FileDtoExtensions
{
    public static FileDto ToDto(this Domain.Entities.File file)
    {
        return new FileDto
        (
            file.Id,
            file.OwnerUserId,
            file.FileDetails.ToDto(),
            file.MarkedForDeletionAt,
            file.DeletedAt,
            file.CreatedAt
        );
    }
}