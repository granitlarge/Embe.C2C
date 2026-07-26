using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Embe.C2C.Application.Abstractions.Services;
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

    private static string GetBlobName(string imageName, ImageSize size)
    {
        return $"{imageName}{(size == ImageSize.Original ? "" : $"-{Enum.GetName(size)}")}";
    }

    public async Task DeleteImageAsync(string name, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        foreach (var imageSize in Enum.GetValues<ImageSize>())
        {
            var blobClient = blobContainerClient.GetBlobClient(GetBlobName(name, imageSize));
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
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
        ImageSize size,
        FilePermissions permissions,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default
    )
    {
        var path = GetBlobName(fileName,  size);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(path);
        var blobSasPermissions =
            (permissions.HasFlag(FilePermissions.Read) ? BlobSasPermissions.Read : 0) |
            (permissions.HasFlag(FilePermissions.Write) ? BlobSasPermissions.Write : 0);

        if (permissions.HasFlag(FilePermissions.Read) && !permissions.HasFlag(FilePermissions.Write))
        {
            var exists = await blobClient.ExistsAsync(cancellationToken);
            if (!exists)
            {
                return null;
            }
        }

        var sasUri = blobClient.GenerateSasUri(blobSasPermissions, DateTimeOffset.UtcNow.Add(lifetime));
        return sasUri.ToString();
    }

    public async Task<string> GetImageUrlAsync(string name, ImageSize size, CancellationToken cancellationToken)
    {
        var path = GetBlobName(name, size);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var bc = blobContainerClient.GetBlobClient(path);
        return bc.Uri.ToString();
    }

    private static string GetBlobNameFromBlobUrl(string blobUrl)
    {
        var blobUri = new BlobUriBuilder(new Uri(blobUrl));
        return blobUri.BlobName;
    }

    public async Task<bool> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var blobName = GetBlobNameFromBlobUrl(url);
        var exists = await blobContainerClient.GetBlobClient(blobName).ExistsAsync(cancellationToken);
        return exists;
    }

    public async Task<bool> ExistsAsync(string imageName, CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobName(imageName, ImageSize.Original);
        var blobContainerClient = await GetBlobContainerClientAsync(cancellationToken);
        var exists = await blobContainerClient.GetBlobClient(blobName).ExistsAsync(cancellationToken);
        return exists;
    }

    private async Task<BlobContainerClient> GetBlobContainerClientAsync(CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        return blobContainerClient;
    }

    public async Task<IUploadImageResult> UploadImageAsync
    (
        byte[] data, 
        int newWidth,
        int newHeight,
        int cropOffsetX,
        int cropOffsetY,
        CancellationToken cancellationToken
    )
    {
        var bcc = await GetBlobContainerClientAsync(cancellationToken);
        var imageName = Guid.CreateVersion7().ToString();
        var sourceBlobName = GetBlobName(imageName, ImageSize.Original);
        var bc = bcc.GetBlobClient(sourceBlobName);

        using var image = new MagickImage();
        image.Read(data);
        image.Crop(new MagickGeometry(cropOffsetX, cropOffsetY, (uint)newWidth, (uint)newHeight));
        image.ResetPage();
        image.Format = MagickFormat.WebP;

        using (var destBlobStream = await bc.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken))
        {
            await image.WriteAsync(destBlobStream, cancellationToken);
        }

        var originalUrl = bc.Uri.ToString();
        var urls = await Task.WhenAll([
            ResizeImageAsync(ImageSize.Large),
            ResizeImageAsync(ImageSize.Medium),
            ResizeImageAsync(ImageSize.Small)
        ]);

        var largeUrl = urls[0];
        var mediumUrl = urls[1];
        var smallUrl = urls[2];

        return new UploadImageResult(originalUrl, largeUrl, mediumUrl, smallUrl, imageName);

        async Task<string> ResizeImageAsync(ImageSize imageSize)
        {
            using var scaledImage = new MagickImage(image);
            var destBlobClient = bcc.GetBlobClient(GetBlobName(imageName, imageSize));
            var factor = imageSize switch
            {
                ImageSize.Large => 0.5,
                ImageSize.Medium => .25,
                ImageSize.Small => .125,
                _ => throw new NotImplementedException()
            };

            scaledImage.Resize(new MagickGeometry(0, 0, (uint)(factor * scaledImage.Width), (uint)(factor * scaledImage.Height)));
            using var destBlobStream = await destBlobClient.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);
            scaledImage.Format = MagickFormat.WebP;
            await scaledImage.WriteAsync(destBlobStream, cancellationToken);
            return destBlobClient.Uri.ToString();
        }
    }
}

internal record UploadImageResult(string OriginalUrl, string LargeUrl, string MediumUrl, string SmallUrl, string Name) : IUploadImageResult;