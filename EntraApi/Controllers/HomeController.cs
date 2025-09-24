using System.Data;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EntraApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EntraApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HomeController : ControllerBase
{
  private readonly IConfiguration _configuration;

  public HomeController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  [HttpGet("employees-config")]
  public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesFromConfig()
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      return Problem("Connection string 'DefaultConnection' not configured.", statusCode: 500);
    }

    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    var employees = await QueryEmployeesAsync(connection);
    return Ok(employees);
  }

  [HttpGet("employees-kv-msi")]
  public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesFromKeyVaultUsingMsi()
  {
    var vaultUrl = _configuration["KeyVault:VaultUrl"];
    var secretName = _configuration["KeyVault:SqlConnectionStringSecretName"];
    if (string.IsNullOrWhiteSpace(vaultUrl) || string.IsNullOrWhiteSpace(secretName))
    {
      return Problem("Key Vault 'VaultUrl' or 'SqlConnectionStringSecretName' not configured.", statusCode: 500);
    }

    var credential = new DefaultAzureCredential();
    var secretClient = new SecretClient(new Uri(vaultUrl), credential);
    KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);
    var connectionString = secret.Value;

    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    var employees = await QueryEmployeesAsync(connection);
    return Ok(employees);
  }

  [HttpGet("employees-kv-uri")]
  public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesFromKeyVaultUsingSecretUri()
  {
    var secretUri = _configuration["KeyVault:SqlConnectionStringSecretUri"];
    if (string.IsNullOrWhiteSpace(secretUri))
    {
      return Problem("Key Vault 'SqlConnectionStringSecretUri' not configured.", statusCode: 500);
    }

    var identifier = new KeyVaultSecretIdentifier(new Uri(secretUri));
    var credential = new DefaultAzureCredential();
    var secretClient = new SecretClient(identifier.VaultUri, credential);
    KeyVaultSecret secret = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
    var connectionString = secret.Value;

    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    var employees = await QueryEmployeesAsync(connection);
    return Ok(employees);
  }

  [HttpGet("employees-msi-db")]
  public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesUsingMsiToSql()
  {
    // Connection string without user/password. Example provided in appsettings: 'AzureSqlMsi'
    var baseConnectionString = _configuration.GetConnectionString("AzureSqlMsi");
    if (string.IsNullOrWhiteSpace(baseConnectionString))
    {
      return Problem("Connection string 'AzureSqlMsi' not configured.", statusCode: 500);
    }

    var credential = new DefaultAzureCredential();
    // Azure SQL Database resource scope
    var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
    AccessToken token = await credential.GetTokenAsync(tokenRequestContext);

    await using var connection = new SqlConnection(baseConnectionString);
    connection.AccessToken = token.Token;
    await connection.OpenAsync();
    var employees = await QueryEmployeesAsync(connection);
    return Ok(employees);
  }

  private static async Task<List<Employee>> QueryEmployeesAsync(SqlConnection connection)
  {
    var employees = new List<Employee>();
    await using var command = connection.CreateCommand();
    command.CommandText = "SELECT id, name, title, email FROM employee";
    command.CommandType = CommandType.Text;

    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      employees.Add(new Employee
      {
        Id = reader.GetInt32(0),
        Name = reader.IsDBNull(1) ? null : reader.GetString(1),
        Title = reader.IsDBNull(2) ? null : reader.GetString(2),
        Email = reader.IsDBNull(3) ? null : reader.GetString(3)
      });
    }
    return employees;
  }
}

