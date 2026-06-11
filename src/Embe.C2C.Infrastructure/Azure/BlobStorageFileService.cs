using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Embe.C2C.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure.Azure;

public class BlobStorageFileService : IFileService
{
    private static readonly string _containerName = "files";
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageFileService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage");
        _blobServiceClient = new BlobServiceClient(connectionString, new BlobClientOptions(BlobClientOptions.ServiceVersion.V2025_11_05));
    }

    public async Task DeleteFileByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(name);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task DeleteFileByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        await new BlobClient(new Uri(url)).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> GenerateFileSasUrlAsync(string fileName, TimeSpan lifetime, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            throw new FileNotFoundException($"Blob with name '{fileName}' not found.");
        }
        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(lifetime));
        return sasUri.ToString();
    }

    public async Task<IUploadFileResult> UploadFileAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(Guid.CreateVersion7().ToString());

        using var stream = new MemoryStream(content);
        await blobClient.UploadAsync(stream, cancellationToken);
        return new UploadFileResult(blobClient.Uri.ToString(), blobClient.Name);
    }
}

internal record UploadFileResult(string Url, string Name) : IUploadFileResult;