using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos.Read.ValueObjects;

public record ImageDetailsDto
(
    string? Url,
    string? Name,
    string MimeType,
    int? Order,
    ImageStatus Status
);

public static class ImageDetailsDtoExtensions
{
    public static async Task<ImageDetailsDto> ToDtoAsync(this ImageDetails fileDetails, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        var url = await fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, fileDetails.Status, Abstractions.Services.FilePermissions.Read, cancellationToken);
        return new ImageDetailsDto(url, fileDetails.Name, fileDetails.MimeType, fileDetails.Order, fileDetails.Status);
    }
}