using Embe.C2C.Application.Dtos.ValueObjects;

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
    public static async Task<FileDto> ToDto(this Domain.Entities.File file, IFileUrlGenerator fileUrlGenerator)
    {
        return new FileDto
        (
            file.Id,
            file.OwnerUserId,
            await file.FileDetails.ToDto(fileUrlGenerator),
            file.MarkedForDeletionAt,
            file.DeletedAt,
            file.CreatedAt
        );
    }
}