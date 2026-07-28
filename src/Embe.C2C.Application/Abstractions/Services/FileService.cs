
namespace Embe.C2C.Application.Abstractions.Services;

public interface IUploadImageResult
{
    string OriginalUrl { get; }
    string LargeUrl { get; }
    string MediumUrl { get; }
    string SmallUrl { get; }
    string Name { get; }
}

public interface IImageService
{
    Task<string?> GenerateImageSasUrlAsync
    (
        string name,
        ImageSize imageSize,
        FilePermissions permissions,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default
    );

    Task<string> GetImageUrlAsync
    (
        string name,
        ImageSize size,
        CancellationToken cancellationToken
    );

    Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string imageName, CancellationToken cancellationToken = default);

    Task DeleteImageAsync(string name, CancellationToken cancellationToken = default);

    Task DeleteImageByUrlAsync(string url, CancellationToken cancellationToken = default);

    Task<IUploadImageResult> UploadImageAsync
    (
        byte[] data,
        double aspect,
        int cropOffsetX,
        int cropOffsetY,
        CancellationToken cancellationToken
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