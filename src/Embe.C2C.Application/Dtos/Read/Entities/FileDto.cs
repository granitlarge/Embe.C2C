using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.Entities;

public record FileDto
(
    Guid Id,
    Guid OwnerUserId,
    FileDetailsDto? FileDetails,
    DateTimeOffset? MarkedForDeletionAt,
    DateTimeOffset? DeletedAt,
    DateTimeOffset? CreatedAt
);

public static class FileDtoExtensions
{
    public static async Task<FileDto> ToDtoAsync(this Domain.Entities.File file, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        return new FileDto
        (
            file.Id,
            file.OwnerUserId,
            await file.FileDetails.ToDtoAsync(fileUrlGenerator, cancellationToken),
            file.MarkedForDeletionAt,
            file.DeletedAt,
            file.CreatedAt
        );
    }
}