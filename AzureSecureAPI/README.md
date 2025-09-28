# Azure Secure API

This is a .NET 9 Web API with Azure AD authentication, Swagger integration, and various Azure services including Key Vault and Blob Storage.

## Features

- **Authentication**: Microsoft Entra ID (Azure AD) authentication
- **Swagger**: Fully integrated with OAuth2 authentication
- **Database Access**: Both ADO.NET and Entity Framework Core implementations
- **Azure Key Vault**: Multiple methods to retrieve secrets (MSI, Client Credentials)
- **Azure Blob Storage**: File upload/download with SAS token, Key Vault, and MSI authentication
- **Managed Identity**: Support for Azure Managed Service Identity (MSI)

## Prerequisites

1. .NET 9 SDK
2. Azure Subscription
3. Azure AD App Registration
4. SQL Database (Azure SQL or SQL Server)
5. Azure Key Vault
6. Azure Storage Account

## Configuration

Update the `appsettings.json` file with your Azure resources:

```json
{
  "AzureAd": {
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },
  "BlobStorage": {
    "ConnectionString": "YOUR_BLOB_CONNECTION_STRING",
    "AccountName": "YOUR_STORAGE_ACCOUNT_NAME",
    "SasToken": "YOUR_SAS_TOKEN"
  },
  "KeyVault": {
    "VaultUri": "https://YOUR_VAULT_NAME.vault.azure.net/"
  }
}
```

## Azure AD App Registration Setup

1. Register an application in Azure AD
2. Configure the following:
   - Redirect URI: `https://localhost:5001/swagger/oauth2-redirect.html`
   - API Permissions: Add necessary permissions
   - Expose an API: Create a scope like `api://YOUR_CLIENT_ID/access_as_user`
   - Client Secret: Create a client secret for Key Vault access

## Key Vault Secrets

Create the following secrets in your Key Vault:
- `DatabaseConnectionString`: Connection string for SQL database
- `BlobStorageConnectionString`: Connection string for blob storage

## API Endpoints

### HomeController (ADO.NET)
- `GET /api/home/employees` - Get employees using connection string from appsettings
- `GET /api/home/employees-keyvault-msi` - Get employees using Key Vault MSI
- `GET /api/home/employees-keyvault-url` - Get employees using Key Vault with client credentials
- `GET /api/home/employees-db-msi` - Get employees using database MSI authentication

### Home2Controller (Entity Framework)
- `GET /api/home2/employees` - Get all employees
- `GET /api/home2/employees/{id}` - Get employee by ID
- `POST /api/home2/employees` - Create new employee
- `PUT /api/home2/employees/{id}` - Update employee
- `DELETE /api/home2/employees/{id}` - Delete employee

### FileController (Blob Storage)
- `POST /api/file/upload-sas` - Upload file using SAS token
- `POST /api/file/upload-keyvault` - Upload file using Key Vault connection
- `POST /api/file/upload-msi` - Upload file using Managed Identity
- `GET /api/file/list` - List all files
- `GET /api/file/download/{fileName}` - Download file
- `DELETE /api/file/delete/{fileName}` - Delete file

## Running the Application

1. Clone the repository
2. Update the configuration in `appsettings.json`
3. Run Entity Framework migrations (if using EF):
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
5. Navigate to `https://localhost:5001/swagger` to access Swagger UI
6. Click "Authorize" and login with your Azure AD credentials

## Database Setup

Create the Employee table in your SQL database:

```sql
CREATE TABLE Employee (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Department NVARCHAR(50) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL
);

-- Insert sample data
INSERT INTO Employee (Name, Email, Department, Salary) VALUES
('John Doe', 'john.doe@example.com', 'IT', 75000),
('Jane Smith', 'jane.smith@example.com', 'HR', 65000),
('Bob Johnson', 'bob.johnson@example.com', 'Sales', 70000);
```

## Security Considerations

- Never commit sensitive information like connection strings or secrets to source control
- Use Azure Key Vault for all sensitive configuration
- Enable Managed Identity for production deployments
- Implement proper RBAC in Azure for resource access
- Use HTTPS in production
- Implement proper error handling to avoid exposing sensitive information

## Troubleshooting

1. **Authentication Issues**: Ensure your Azure AD app registration is properly configured
2. **Key Vault Access**: Verify the service principal or managed identity has proper access policies
3. **Database Connection**: Check firewall rules and connection string format
4. **Blob Storage**: Ensure the storage account allows the authentication method you're using