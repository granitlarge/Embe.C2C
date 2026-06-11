namespace Embe.C2C.Application.Abstractions.Services;

public interface IUploadFileResult
{
    string Url { get; }
    string Name { get; }
}

public interface IFileService
{
    Task<IUploadFileResult> UploadFileAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default);
    Task<string> GenerateFileSasUrlAsync(string fileName, TimeSpan lifetime, CancellationToken cancellationToken = default);
    Task DeleteFileByNameAsync(string name, CancellationToken cancellationToken = default);
    Task DeleteFileByUrlAsync(string url, CancellationToken cancellationToken = default);
}