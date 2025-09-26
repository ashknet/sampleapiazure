namespace Emr.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string containerName, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string blobUrl, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string blobUrl, CancellationToken cancellationToken = default);
    Task<string> GetSasTokenAsync(string blobUrl, TimeSpan expiry, BlobSasPermissions permissions, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string blobUrl, CancellationToken cancellationToken = default);
    Task<BlobMetadata> GetMetadataAsync(string blobUrl, CancellationToken cancellationToken = default);
}

public class BlobMetadata
{
    public long ContentLength { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTimeOffset? LastModified { get; set; }
    public string? ETag { get; set; }
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}

public enum BlobSasPermissions
{
    Read = 1,
    Write = 2,
    Delete = 4,
    List = 8
}