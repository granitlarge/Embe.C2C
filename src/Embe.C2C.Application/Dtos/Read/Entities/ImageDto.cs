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

public class ImageDtoMapper
{
    private readonly IFileUrlGenerator _fileUrlGenerator;

    public ImageDtoMapper(IFileUrlGenerator fileUrlGenerator)
    {
        _fileUrlGenerator = fileUrlGenerator;
    }

    public async Task<ImageDto> ToDtoAsync(Domain.Entities.Image image, CancellationToken cancellationToken = default)
    {
        return new ImageDto
        (
            image.Id,
            image.OwnerUserId,
            await image.ImageDetails.ToDtoAsync(_fileUrlGenerator, cancellationToken),
            image.MarkedForDeletionAt,
            image.DeletedAt,
            image.CreatedAt
        );
    }
}