namespace Embe.C2C.Application.Abstractions.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default);
#warning this should not throw if the file does not exist, it should just return
    Task DeleteFileAsync(string url, CancellationToken cancellationToken = default);
}