using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Abstractions.Services;

public interface IUploadImageResult
{
    string Url { get; }
    string Name { get; }
}

public interface IImageService
{
    Task<IUploadImageResult> UploadImageAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default);
    Task<string?> GenerateImageSasUrlAsync
    (
        string name,
        ImageStatus status,
        ImageSize imageSize,
        FilePermissions permissions,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default
    );

    Task<string> GetImageUrlAsync
    (
        string name,
        ImageStatus status,
        ImageSize size,
        CancellationToken cancellationToken
    );

    Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string imageName, ImageStatus imageStatus, ImageSize size, CancellationToken cancellationToken = default);

    Task DeleteImageAsync(string name, ImageStatus status, CancellationToken cancellationToken = default);

    Task DeleteImageByUrlAsync(string url, CancellationToken cancellationToken = default);

    Task MoveImageAsync(string name, ImageStatus oldStatus, ImageStatus newStatus, CancellationToken cancellationToken = default);

    Task MoveImageAsync(string fromUrl, string toUrl, CancellationToken cancellationToken = default);

    Task ReformatImageAsWebpAsync(string url, CancellationToken cancellationToken = default);

    Task<string> CropImageAsync
    (
        string url,
        int newWidth,
        int newHeight,
        int offsetX,
        int offsetY,
        string imageName,
        ImageStatus imageStatus,
        ImageSize size,
        CancellationToken cancellationToken = default
    );

    Task<string> ScaleImageAsync
    (
        string url,
        double factor,
        string imageName,
        ImageStatus imageStatus,
        ImageSize imageSize,
        CancellationToken cancellationToken = default
    );

}

[Flags]
public enum FilePermissions
{
    Read = 1,
    Write = 2,
}

public enum ImageSize
{
    Original,
    Large,
    Medium,
    Small
}