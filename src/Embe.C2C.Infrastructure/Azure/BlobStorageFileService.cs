using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.ValueObjects;
using ImageMagick;
using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure.Azure;

public class BlobStorageImageService : IImageService
{
    private static readonly string _containerName = "images";
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageImageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorageBlobs") ?? configuration.GetValue<string>("AzureStorageBlobs");
        _blobServiceClient = new BlobServiceClient(connectionString, new BlobClientOptions(BlobClientOptions.ServiceVersion.V2025_11_05));
    }

    private static string GetBlobName(string imageName, ImageStatus status, ImageSize size)
    {
        var pathElement = status switch
        {
            ImageStatus.Accepted => "accepted",
            ImageStatus.Rejected => "rejected",
            ImageStatus.Pending => "pending",
            _ => throw new NotImplementedException()
        };
        return $"{pathElement}/{imageName}{(size == ImageSize.Original ? "" : Enum.GetName(size))}";
    }

    public async Task DeleteImageAsync(string name, ImageStatus status, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(GetBlobName(name, status, ImageSize.Original));
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task DeleteImageByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobNameFromBlobUrl(url);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var bc = blobContainerClient.GetBlobClient(blobName);
        await bc.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string?> GenerateImageSasUrlAsync
    (
        string fileName,
        ImageStatus status,
        ImageSize size,
        FilePermissions permissions,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default
    )
    {
        var path = GetBlobName(fileName, status, size);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(path);
        var blobSasPermissions =
            (permissions.HasFlag(FilePermissions.Read) ? BlobSasPermissions.Read : 0) |
            (permissions.HasFlag(FilePermissions.Write) ? BlobSasPermissions.Write : 0);
        var sasUri = blobClient.GenerateSasUri(blobSasPermissions, DateTimeOffset.UtcNow.Add(lifetime));
        return sasUri.ToString();
    }

    public async Task<string> GetImageUrlAsync(string name, ImageStatus status, ImageSize size, CancellationToken cancellationToken)
    {
        var path = GetBlobName(name, status, size);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var bc = blobContainerClient.GetBlobClient(path);
        return bc.Uri.ToString();
    }

    public async Task MoveImageAsync(string name, ImageStatus oldStatus, ImageStatus newStatus, CancellationToken cancellationToken = default)
    {
        var bcc = await GetBlobContainerClientAsync(cancellationToken);
        var oldBlobPath = GetBlobName(name, oldStatus, ImageSize.Original);
        var bc = bcc.GetBlobClient(oldBlobPath);
        var content = await bc.DownloadContentAsync(cancellationToken);
        var newBlobClient = bcc.GetBlobClient(GetBlobName(name, newStatus, ImageSize.Original));
        await newBlobClient.UploadAsync(content.Value.Content, cancellationToken);
    }

    private static string GetBlobNameFromBlobUrl(string blobUrl)
    {
        var blobUri = new BlobUriBuilder(new Uri(blobUrl));
        return blobUri.BlobName;
    }

    public async Task MoveImageAsync(string fromUrl, string toUrl, CancellationToken cancellationToken = default)
    {
        var fromBlobName = GetBlobNameFromBlobUrl(fromUrl);
        var toBlobName = GetBlobNameFromBlobUrl(toUrl);

        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);

        var fromBc = blobContainerClient.GetBlobClient(fromBlobName);
        var toBc = blobContainerClient.GetBlobClient(toBlobName);

        using (var fromStream = await fromBc.OpenReadAsync(cancellationToken: cancellationToken))
        {
            using var toStream = await toBc.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);
            await fromStream.CopyToAsync(toStream, cancellationToken);
        }
#warning we're not copying the properties (such as Content-Type) from the original blob to the new blob, we should do that
        await fromBc.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<IUploadImageResult> UploadImageAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(Guid.CreateVersion7().ToString());

        using var stream = new MemoryStream(content);
        await blobClient.UploadAsync(stream, cancellationToken);
        return new UploadImageResult(blobClient.Uri.ToString(), blobClient.Name);
    }

    public async Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobName = GetBlobNameFromBlobUrl(url);
        var exists = await blobContainerClient.GetBlobClient(blobName).ExistsAsync(cancellationToken);
        return exists;
    }

    public async Task<bool> ExistsAsync(string imageName, ImageStatus imageStatus, ImageSize size, CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobName(imageName, imageStatus, size);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var exists = await blobContainerClient.GetBlobClient(blobName).ExistsAsync(cancellationToken);
        return exists;
    }

    public async Task ReformatImageAsWebpAsync(string url, CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobNameFromBlobUrl(url);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        var exists = await blobClient.ExistsAsync(cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException($"Blob with url {url} does not exist.");
        }

        using var blobReadStream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        using var blobWriteStream = await blobClient.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);
        using var image = new MagickImage();
        await image.ReadAsync(blobReadStream, cancellationToken);
        image.Format = MagickFormat.WebP;
        await image.WriteAsync(blobWriteStream, cancellationToken);
#warning update the content-type of the blob
    }

    public async Task<string> CropImageAsync
    (
        string url,
        int newWidth,
        int newHeight,
        int offsetX,
        int offsetY,
        string imageName,
        ImageStatus imageStatus,
        ImageSize imageSize,
        CancellationToken cancellationToken = default
    )
    {
        if (newWidth < 1)
            throw new ArgumentException("width must be greater or equal to 1", nameof(newWidth));
        if (newHeight < 1)
            throw new ArgumentException("height must be greater or equal to 1", nameof(newHeight));
        if (offsetX < 0)
            throw new ArgumentException("offsetX must be greater or equal to 0", nameof(offsetX));
        if (offsetY < 0)
            throw new ArgumentException("offsetY must be greater or equal to 0", nameof(offsetY));

        var sourceBlobName = GetBlobNameFromBlobUrl(url);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var sourceBlobCLient = blobContainerClient.GetBlobClient(sourceBlobName);
        var sourceExists = await sourceBlobCLient.ExistsAsync(cancellationToken);
        if (!sourceExists)
        {
            throw new InvalidOperationException($"Blob with url {url} does not exist.");
        }

        var destBlobName = GetBlobName(imageName, imageStatus, imageSize);
        var destBlobClient = blobContainerClient.GetBlobClient(destBlobName);

        using var image = new MagickImage();
        using (var sourceBlobReadStream = await sourceBlobCLient.OpenReadAsync(cancellationToken: cancellationToken))
        {
            await image.ReadAsync(sourceBlobReadStream, cancellationToken);
        }
        image.Crop(new MagickGeometry(offsetX, offsetY, (uint)newWidth, (uint)newHeight));
        image.ResetPage();
        image.Format = MagickFormat.WebP;
        using var destBlobStream = await destBlobClient.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);
        await image.WriteAsync(destBlobStream, cancellationToken);

        return destBlobClient.Uri.ToString();
    }

    public async Task<string> ScaleImageAsync
    (
        string url,
        double factor,
        string imageName,
        ImageStatus imageStatus,
        ImageSize imageSize,
        CancellationToken cancellationToken = default
    )
    {
        if (factor <= 0)
            throw new ArgumentException("factor must be greater than 0", nameof(factor));

        var sourceBlobName = GetBlobNameFromBlobUrl(url);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var sourceBlobCLient = blobContainerClient.GetBlobClient(sourceBlobName);
        var sourceExists = await sourceBlobCLient.ExistsAsync(cancellationToken);
        if (!sourceExists)
        {
            throw new InvalidOperationException($"Blob with url {url} does not exist.");
        }

        var destBlobName = GetBlobName(imageName, imageStatus, imageSize);
        var destBlobClient = blobContainerClient.GetBlobClient(destBlobName);

        using var image = new MagickImage();
        using (var sourceBlobReadStream = await sourceBlobCLient.OpenReadAsync(cancellationToken: cancellationToken))
        {
            await image.ReadAsync(sourceBlobReadStream, cancellationToken);
        }
        image.Resize(new MagickGeometry(0, 0, (uint)(factor * image.Width), (uint)(factor * image.Height)));
        using var destBlobStream = await destBlobClient.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);
        image.Format = MagickFormat.WebP;
        await image.WriteAsync(destBlobStream, cancellationToken);
        return destBlobClient.Uri.ToString();
    }

    private async Task<BlobContainerClient> GetBlobContainerClientAsync(CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        return blobContainerClient;
    }

}

internal record UploadImageResult(string Url, string Name) : IUploadImageResult;