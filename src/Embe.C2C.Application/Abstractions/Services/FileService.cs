namespace Embe.C2C.Application.Abstractions.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string url, CancellationToken cancellationToken = default);
}