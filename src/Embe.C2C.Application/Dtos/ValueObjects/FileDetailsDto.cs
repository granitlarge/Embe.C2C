namespace Embe.C2C.Application.Dtos.ValueObjects;

public record FileDetailsDto
(
    string Url,
    string MimeType
);

public static class FileDetailsDtoExtensions
{
    public static FileDetailsDto ToDto(this Domain.ValueObjects.FileDetails fileDetails)
    {
        return new FileDetailsDto(fileDetails.Url, fileDetails.MimeType);
    }
}