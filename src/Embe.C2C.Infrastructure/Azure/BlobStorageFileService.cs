using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure.Azure;

public class BlobStorageImageService : IImageService
{
    private static readonly string _containerName = "images";
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageImageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage");
        _blobServiceClient = new BlobServiceClient(connectionString, new BlobClientOptions(BlobClientOptions.ServiceVersion.V2025_11_05));
    }

    private static string GetImagePath(string imageName, ImageStatus status)
    {
        var pathElement = status switch
        {
            ImageStatus.Accepted => "accepted",
            ImageStatus.Rejected => "rejected",
            ImageStatus.Pending => "pending",
            _ => throw new NotImplementedException()
        };
        return $"{pathElement}/{imageName}";
    }

    public async Task DeleteImageAsync(string name, ImageStatus status, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(GetImagePath(name, status));
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task DeleteImageByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(url));
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string?> GenerateImageSasUrlAsync
    (
        string fileName,
        ImageStatus status,
        FilePermissions permissions,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default
    )
    {
        var path = GetImagePath(fileName, status);
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(path);
        var blobSasPermissions =
            (permissions.HasFlag(FilePermissions.Read) ? BlobSasPermissions.Read : 0) |
            (permissions.HasFlag(FilePermissions.Write) ? BlobSasPermissions.Write : 0);
        var sasUri = blobClient.GenerateSasUri(blobSasPermissions, DateTimeOffset.UtcNow.Add(lifetime));
        return sasUri.ToString();
    }

    public async Task MoveImageAsync(string name, ImageStatus oldStatus, ImageStatus newStatus, CancellationToken cancellationToken = default)
    {
        var bcc = _blobServiceClient.GetBlobContainerClient(_containerName);
        await bcc.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var oldBlobPath = GetImagePath(name, oldStatus);
        var bc = bcc.GetBlobClient(oldBlobPath);
        var content = await bc.DownloadContentAsync(cancellationToken);
        var newBlobClient = bcc.GetBlobClient(GetImagePath(name, newStatus));
        await newBlobClient.UploadAsync(content.Value.Content, cancellationToken);
    }

    public async Task<IUploadImageResult> UploadImageAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(Guid.CreateVersion7().ToString());

        using var stream = new MemoryStream(content);
        await blobClient.UploadAsync(stream, cancellationToken);
        return new UploadImageResult(blobClient.Uri.ToString(), blobClient.Name);
    }
}

internal record UploadImageResult(string Url, string Name) : IUploadImageResult;