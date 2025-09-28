using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureSecureAPI.Models;
using System.Data;

namespace AzureSecureAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Method 1: Get employees using connection string from appsettings.json
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = new List<Employee>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT Id, Name FROM Employee", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new Employee
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Department = reader.GetString(3),
                                    Salary = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees from database");
                return StatusCode(500, "An error occurred while fetching employees");
            }
        }

        // Method 2: Get connection string from Key Vault using MSI and fetch employees
        [HttpGet("employees-keyvault-msi")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesKeyVaultMSI()
        {
            var employees = new List<Employee>();

            try
            {
                // Connect to Key Vault using Managed Identity
                var keyVaultUrl = _configuration["KeyVault:VaultUri"];
                var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
                
                // Get connection string from Key Vault
                KeyVaultSecret secret = await secretClient.GetSecretAsync("DatabaseConnectionString");
                string connectionString = secret.Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, Email, Department, Salary FROM Employee", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new Employee
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Department = reader.GetString(3),
                                    Salary = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees using Key Vault MSI");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Method 3: Get connection string from Key Vault using URL (Client Credentials) and fetch employees
        [HttpGet("employees-keyvault-url")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesKeyVaultURL()
        {
            var employees = new List<Employee>();

            try
            {
                // Connect to Key Vault using Client Credentials
                var keyVaultUrl = _configuration["KeyVault:VaultUri"];
                var tenantId = _configuration["AzureAd:TenantId"];
                var clientId = _configuration["AzureAd:ClientId"];
                var clientSecret = _configuration["AzureAd:ClientSecret"]; // Add this to appsettings
                
                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);
                
                // Get connection string from Key Vault
                KeyVaultSecret secret = await secretClient.GetSecretAsync("DatabaseConnectionString");
                string connectionString = secret.Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, Email, Department, Salary FROM Employee", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new Employee
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Department = reader.GetString(3),
                                    Salary = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees using Key Vault URL");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Method 4: Connect to database using MSI directly
        [HttpGet("employees-db-msi")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesDbMSI()
        {
            var employees = new List<Employee>();

            try
            {
                // Connection string using MSI authentication
                var server = _configuration["Database:Server"];
                var database = _configuration["Database:DatabaseName"];
                var connectionString = $"Server={server};Database={database};Authentication=Active Directory Managed Identity;TrustServerCertificate=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("SELECT Id, Name, Email, Department, Salary FROM Employee", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new Employee
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Department = reader.GetString(3),
                                    Salary = reader.GetDecimal(4)
                                });
                            }
                        }
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees using database MSI");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}