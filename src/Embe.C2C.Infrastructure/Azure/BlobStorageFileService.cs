using System.Reflection.Metadata;
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
        var connectionString = configuration.GetConnectionString("AzureStorageBlobs") ?? configuration.GetValue<string>("AzureStorageBlobs");
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
        var blobName = GetBlobNameFromBlobUrl(url);
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var bc = blobContainerClient.GetBlobClient(blobName);
        await bc.DeleteIfExistsAsync(cancellationToken: cancellationToken);
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

    public async Task<string> GetImageUrlAsync(string name, ImageStatus status, CancellationToken cancellationToken)
    {
        var path = GetImagePath(name, status);
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var bc = blobContainerClient.GetBlobClient(path);
        return bc.Uri.ToString();
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

    private static string GetBlobNameFromBlobUrl(string blobUrl)
    {
        var pathDirectorySeparatorChars = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        if (blobUrl.StartsWith(pathDirectorySeparatorChars))
        {
            blobUrl = blobUrl[1..];
        }

        if (blobUrl.EndsWith(pathDirectorySeparatorChars))
        {
            blobUrl = blobUrl[^1..];
        }

        var blobNameParts = new Uri(blobUrl).AbsolutePath.Split(pathDirectorySeparatorChars).Skip(3);
        var blobName = string.Join("/", blobNameParts);
        return blobName;
    }

    public async Task MoveImageAsync(string fromUrl, string toUrl, CancellationToken cancellationToken = default)
    {
        var fromBlobName = GetBlobNameFromBlobUrl(fromUrl);
        var toBlobName = GetBlobNameFromBlobUrl(toUrl);

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var fromBc = blobContainerClient.GetBlobClient(fromBlobName);
        var toBc = blobContainerClient.GetBlobClient(toBlobName);

        Console.WriteLine("FromBlobName: " + fromBlobName);
        Console.WriteLine("ToBlobName: " + toBlobName);
        Console.WriteLine("FromBlob Exists: " + await fromBc.ExistsAsync(cancellationToken));
        Console.WriteLine("ToBlob Exists: " + await toBc.ExistsAsync(cancellationToken));
        using var fromStream = await fromBc.OpenReadAsync(cancellationToken: cancellationToken);
        using var toStream = await toBc.OpenWriteAsync(overwrite: true, cancellationToken: cancellationToken);

        await fromStream.CopyToAsync(toStream, cancellationToken);
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