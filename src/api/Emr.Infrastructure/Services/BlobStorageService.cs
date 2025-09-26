using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Emr.Application.Common.Interfaces;

namespace Emr.Infrastructure.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string containerName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var blobName = $"{Guid.NewGuid()}/{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders }, cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        var response = await blobClient.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToStream();
    }

    public async Task<bool> DeleteAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public Task<string> GetSasTokenAsync(string blobUrl, TimeSpan expiry, Application.Common.Interfaces.BlobSasPermissions permissions, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        
        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException("Cannot generate SAS token for this blob");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
        };

        // Map custom permissions to Azure permissions
        var azurePermissions = (Azure.Storage.Sas.BlobSasPermissions)0; // Initialize with no permissions
        if (permissions.HasFlag(Application.Common.Interfaces.BlobSasPermissions.Read))
            azurePermissions |= Azure.Storage.Sas.BlobSasPermissions.Read;
        if (permissions.HasFlag(Application.Common.Interfaces.BlobSasPermissions.Write))
            azurePermissions |= Azure.Storage.Sas.BlobSasPermissions.Write | Azure.Storage.Sas.BlobSasPermissions.Create | Azure.Storage.Sas.BlobSasPermissions.Add;
        if (permissions.HasFlag(Application.Common.Interfaces.BlobSasPermissions.Delete))
            azurePermissions |= Azure.Storage.Sas.BlobSasPermissions.Delete;
        
        sasBuilder.SetPermissions(azurePermissions);

        return Task.FromResult(blobClient.GenerateSasUri(sasBuilder).ToString());
    }

    public async Task<bool> ExistsAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }

    public async Task<BlobMetadata> GetMetadataAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        return new BlobMetadata
        {
            ContentLength = properties.Value.ContentLength,
            ContentType = properties.Value.ContentType,
            LastModified = properties.Value.LastModified,
            ETag = properties.Value.ETag.ToString(),
            Metadata = properties.Value.Metadata
        };
    }
}