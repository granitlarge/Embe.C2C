using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record ImageDetailsDto
(
    string? Url,
    string? LargeUrl,
    string? MediumUrl,
    string? SmallUrl,
    string? Name,
    string MimeType,
    int? Order
);

public static class ImageDetailsDtoExtensions
{
    public static async Task<ImageDetailsDto> ToDtoAsync(this ImageDetails fileDetails, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        var urls = await Task.WhenAll(
            fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, Abstractions.Services.ImageSize.Original, Abstractions.Services.FilePermissions.Read, cancellationToken),
            fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, Abstractions.Services.ImageSize.Large, Abstractions.Services.FilePermissions.Read, cancellationToken),
            fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, Abstractions.Services.ImageSize.Medium, Abstractions.Services.FilePermissions.Read, cancellationToken),
            fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, Abstractions.Services.ImageSize.Small, Abstractions.Services.FilePermissions.Read, cancellationToken)
        );

        var original = urls[0];
        var large = urls[1];
        var medium = urls[2];
        var small = urls[3];

        return new ImageDetailsDto(original, large, medium, small, fileDetails.Name, fileDetails.MimeType, fileDetails.Order);
    }
}