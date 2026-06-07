using Azure.Storage.Blobs;
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

    public async Task DeleteFileAsync(string url, CancellationToken cancellationToken = default)
    {
        await new BlobClient(new Uri(url)).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> UploadFileAsync(byte[] content, string mimeType, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = blobContainerClient.GetBlobClient(Guid.CreateVersion7().ToString());

        using var stream = new MemoryStream(content);
        await blobClient.UploadAsync(stream, cancellationToken);
        return blobClient.Uri.ToString();
    }
}