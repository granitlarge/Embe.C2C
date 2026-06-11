namespace Embe.C2C.Application.Dtos.ValueObjects;

public record FileDetailsDto
(
    string Url,
    string Name,
    string MimeType,
    int Order
);

public static class FileDetailsDtoExtensions
{
    public static async Task<FileDetailsDto> ToDtoAsync(this Domain.ValueObjects.FileDetails fileDetails, IFileUrlGenerator fileUrlGenerator, CancellationToken cancellationToken = default)
    {
        var url = await fileUrlGenerator.GenerateUrlAsync(fileDetails.Name, cancellationToken);
        return new FileDetailsDto(url, fileDetails.Name, fileDetails.MimeType, fileDetails.Order);
    }
}