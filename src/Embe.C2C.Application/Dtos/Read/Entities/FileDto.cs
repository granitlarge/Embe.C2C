using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Dtos.Read.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.Entities;

public record ImageDto
(
    Guid Id,
    Guid OwnerUserId,
    ImageDetailsDto? ImageDetails,
    DateTimeOffset? MarkedForDeletionAt,
    DateTimeOffset? DeletedAt,
    DateTimeOffset? CreatedAt
);

public static class ImageDtoExtensions
{
    public static async Task<ImageDto> ToDtoAsync(this Domain.Entities.Image file, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        return new ImageDto
        (
            file.Id,
            file.OwnerUserId,
            await file.ImageDetails.ToDtoAsync(fileUrlGenerator, cancellationToken),
            file.MarkedForDeletionAt,
            file.DeletedAt,
            file.CreatedAt
        );
    }
}