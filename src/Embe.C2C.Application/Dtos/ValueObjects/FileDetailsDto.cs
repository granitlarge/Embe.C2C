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
    public static async Task<FileDetailsDto> ToDto(this Domain.ValueObjects.FileDetails fileDetails, IFileUrlGenerator fileUrlGenerator)
    {
        var url = await fileUrlGenerator.GenerateUrlAsync(fileDetails.Name);
        return new FileDetailsDto(url, fileDetails.Name, fileDetails.MimeType, fileDetails.Order);
    }
}