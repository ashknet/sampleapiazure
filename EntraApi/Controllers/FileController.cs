using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EntraApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileController : ControllerBase
{
  private readonly IConfiguration _configuration;

  public FileController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  //[HttpPost("upload-sas")]
  //public async Task<IActionResult> UploadWithSas([FromForm] IFormFile file)
  //{
  //  if (file == null || file.Length == 0) return BadRequest("No file uploaded");

  //  var accountUrl = _configuration["Blob:AccountUrl"];
  //  var containerName = _configuration["Blob:ContainerName"];
  //  var sasToken = _configuration["Blob:SasToken"]; // should start with '?'
  //  if (string.IsNullOrWhiteSpace(accountUrl) || string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(sasToken))
  //  {
  //    return Problem("Blob SAS configuration missing.", statusCode: 500);
  //  }

  //  var containerClient = new BlobContainerClient(new Uri($"{accountUrl}/{containerName}{sasToken}"));
  //  await containerClient.CreateIfNotExistsAsync();

  //  var blobClient = containerClient.GetBlobClient(file.FileName);
  //  await using var stream = file.OpenReadStream();
  //  await blobClient.UploadAsync(stream, overwrite: true);
  //  return Ok(new { blob = blobClient.Uri.ToString() });
  //}

  //[HttpPost("upload-kv")]
  //public async Task<IActionResult> UploadWithKeyVaultConnString([FromForm] IFormFile file)
  //{
  //  if (file == null || file.Length == 0) return BadRequest("No file uploaded");

  //  var vaultUrl = _configuration["KeyVault:VaultUrl"];
  //  var secretName = _configuration["KeyVault:BlobConnectionStringSecretName"];
  //  if (string.IsNullOrWhiteSpace(vaultUrl) || string.IsNullOrWhiteSpace(secretName))
  //  {
  //    return Problem("Key Vault blob connection string secret not configured.", statusCode: 500);
  //  }

  //  var credential = new DefaultAzureCredential();
  //  var secretClient = new SecretClient(new Uri(vaultUrl), credential);
  //  var secret = await secretClient.GetSecretAsync(secretName);
  //  var connectionString = secret.Value.Value;

  //  var containerName = _configuration["Blob:ContainerName"];
  //  var containerClient = new BlobContainerClient(connectionString, containerName);
  //  await containerClient.CreateIfNotExistsAsync();

  //  var blobClient = containerClient.GetBlobClient(file.FileName);
  //  await using var stream = file.OpenReadStream();
  //  await blobClient.UploadAsync(stream, overwrite: true);
  //  return Ok(new { blob = blobClient.Uri.ToString() });
  //}

  //[HttpPost("upload-msi")]
  //public async Task<IActionResult> UploadWithMsi([FromForm] IFormFile file)
  //{
  //  if (file == null || file.Length == 0) return BadRequest("No file uploaded");

  //  var accountUrl = _configuration["Blob:AccountUrl"];
  //  var containerName = _configuration["Blob:ContainerName"];
  //  if (string.IsNullOrWhiteSpace(accountUrl) || string.IsNullOrWhiteSpace(containerName))
  //  {
  //    return Problem("Blob account url or container not configured.", statusCode: 500);
  //  }

  //  var credential = new DefaultAzureCredential();
  //  var containerClient = new BlobContainerClient(new Uri($"{accountUrl}/{containerName}"), credential);
  //  await containerClient.CreateIfNotExistsAsync();

  //  var blobClient = containerClient.GetBlobClient(file.FileName);
  //  await using var stream = file.OpenReadStream();
  //  await blobClient.UploadAsync(stream, overwrite: true);
  //  return Ok(new { blob = blobClient.Uri.ToString() });
  //}
}

