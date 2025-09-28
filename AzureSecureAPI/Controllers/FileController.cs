using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Sas;
using Azure.Storage;

namespace AzureSecureAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileController> _logger;

        public FileController(IConfiguration configuration, ILogger<FileController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Method 1: Upload file using SAS token from appsettings.json
        [HttpPost("upload-sas")]
        public async Task<IActionResult> UploadFileSAS(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file to upload");

            try
            {
                var blobConnectionString = _configuration["BlobStorage:ConnectionString"];
                var containerName = _configuration["BlobStorage:ContainerName"];
                var sasToken = _configuration["BlobStorage:SasToken"];

                // Create BlobServiceClient using connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                
                // Ensure container exists
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Generate unique file name
                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Upload file
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders 
                    { 
                        ContentType = file.ContentType 
                    });
                }

                // Return the URL with SAS token
                var blobUrl = $"{blobClient.Uri}?{sasToken}";
                
                return Ok(new 
                { 
                    message = "File uploaded successfully using SAS token", 
                    fileName = fileName,
                    url = blobUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file using SAS token");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Method 2: Upload file using connection string from Key Vault
        [HttpPost("upload-keyvault")]
        public async Task<IActionResult> UploadFileKeyVault(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file to upload");

            try
            {
                // Get connection string from Key Vault
                var keyVaultUrl = _configuration["KeyVault:VaultUri"];
                var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
                
                KeyVaultSecret secret = await secretClient.GetSecretAsync("BlobStorageConnectionString");
                string connectionString = secret.Value;

                var containerName = _configuration["BlobStorage:ContainerName"];

                // Create BlobServiceClient using connection string from Key Vault
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                
                // Ensure container exists
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Generate unique file name
                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Upload file
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders 
                    { 
                        ContentType = file.ContentType 
                    });
                }

                return Ok(new 
                { 
                    message = "File uploaded successfully using Key Vault connection", 
                    fileName = fileName,
                    url = blobClient.Uri.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file using Key Vault");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Method 3: Upload file using Managed Identity (MSI)
        [HttpPost("upload-msi")]
        public async Task<IActionResult> UploadFileMSI(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file to upload");

            try
            {
                // Get storage account name from configuration
                var storageAccountName = _configuration["BlobStorage:AccountName"];
                var containerName = _configuration["BlobStorage:ContainerName"];

                // Create BlobServiceClient using DefaultAzureCredential (MSI)
                var blobServiceClient = new BlobServiceClient(
                    new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                    new DefaultAzureCredential());

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                
                // Ensure container exists
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Generate unique file name
                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Upload file
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders 
                    { 
                        ContentType = file.ContentType 
                    });
                }

                return Ok(new 
                { 
                    message = "File uploaded successfully using Managed Identity", 
                    fileName = fileName,
                    url = blobClient.Uri.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file using MSI");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // List all files in the container
        [HttpGet("list")]
        public async Task<IActionResult> ListFiles()
        {
            try
            {
                var blobConnectionString = _configuration["BlobStorage:ConnectionString"];
                var containerName = _configuration["BlobStorage:ContainerName"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var files = new List<object>();

                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    files.Add(new 
                    {
                        name = blobItem.Name,
                        contentType = blobItem.Properties.ContentType,
                        size = blobItem.Properties.ContentLength,
                        lastModified = blobItem.Properties.LastModified
                    });
                }

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing files");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Download file
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                var blobConnectionString = _configuration["BlobStorage:ConnectionString"];
                var containerName = _configuration["BlobStorage:ContainerName"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                {
                    return NotFound("File not found");
                }

                var properties = await blobClient.GetPropertiesAsync();
                var stream = await blobClient.OpenReadAsync();

                return File(stream, properties.Value.ContentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Delete file
        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            try
            {
                var blobConnectionString = _configuration["BlobStorage:ConnectionString"];
                var containerName = _configuration["BlobStorage:ContainerName"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.DeleteIfExistsAsync();

                if (response.Value)
                {
                    return Ok(new { message = "File deleted successfully" });
                }
                else
                {
                    return NotFound("File not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}