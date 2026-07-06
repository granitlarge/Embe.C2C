namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record ImageDetailsDto
(
    string Url,
    string? Name,
    string MimeType,
    int? Order
);

public static class ImageDetailsDtoExtensions
{
    public static async Task<ImageDetailsDto> ToDtoAsync(this Domain.ValueObjects.ImageDetails fileDetails, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        var url = await fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, cancellationToken);
        return new ImageDetailsDto(url, fileDetails.Name, fileDetails.MimeType, fileDetails.Order);
    }
}