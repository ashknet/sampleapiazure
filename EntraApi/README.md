# EntraApi (.NET 9 Web API)

Secure Web API with Microsoft Entra ID authentication, Swagger OAuth2, SQL via ADO.NET/EF Core, and Azure Blob uploads (SAS, Key Vault, MSI).

## Prerequisites
- .NET 9 SDK
- Azure: App Registration(s), Key Vault, SQL Database, Storage Account

## Configuration
Edit `appsettings.json`:
- AzureAd: set `TenantId`, `ClientId`, optional `Audience`. For Swagger UI, set `AzureAd:Swagger:ClientId` and `AzureAd:Swagger:Scope`.
- KeyVault: set `VaultUrl`, secret names/URIs.
- ConnectionStrings: set `DefaultConnection` and `AzureSqlMsi`.
- Blob: set `AccountUrl`, `ContainerName`, optional `SasToken`.

Minimal keys:
```json
{
  "AzureAd": {
    "TenantId": "<tenant-id>",
    "ClientId": "<api-app-id>",
    "Audience": "api://<api-app-id>",
    "Swagger": {
      "ClientId": "<swagger-client-id>",
      "Scope": "api://<api-app-id>/user_impersonation"
    }
  },
  "KeyVault": {
    "VaultUrl": "https://<vault>.vault.azure.net/",
    "SqlConnectionStringSecretName": "SqlConnectionString",
    "SqlConnectionStringSecretUri": "https://<vault>.vault.azure.net/secrets/SqlConnectionString/<version>",
    "BlobConnectionStringSecretName": "BlobConnectionString"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<server>.database.windows.net,1433;Database=<db>;User Id=<user>;Password=<password>;Encrypt=True;",
    "AzureSqlMsi": "Server=tcp:<server>.database.windows.net,1433;Database=<db>;Encrypt=True;TrustServerCertificate=False;"
  },
  "Blob": {
    "AccountUrl": "https://<account>.blob.core.windows.net",
    "ContainerName": "<container>",
    "SasToken": "?sv=..."
  }
}
```

## Entra ID setup
1. Register API app. Expose scope `user_impersonation`.
2. Register Swagger client app (public). Redirect URIs: `https://localhost:5001/swagger/oauth2-redirect.html`, `http://localhost:5000/swagger/oauth2-redirect.html`.
3. Grant client delegated permission to API scope. Consent.
4. Put IDs and scope into `appsettings.json`.

## Endpoints
- `api/home/employees-config`: ADO.NET using `ConnectionStrings:DefaultConnection`.
- `api/home/employees-kv-msi`: ADO.NET, Key Vault via MSI.
- `api/home/employees-kv-uri`: ADO.NET, Key Vault via secret URI + MSI.
- `api/home/employees-msi-db`: ADO.NET, Azure SQL token via MSI.
- `api/home2/employees-ef`: EF Core query of `employee` table.
- `api/file/upload-sas`: Upload via SAS token.
- `api/file/upload-kv`: Upload using conn string from Key Vault.
- `api/file/upload-msi`: Upload using MSI.

All endpoints require auth. Use Swagger UI login button.

## Run
```bash
dotnet run --project EntraApi
```
Swagger: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`.

## Notes
- MSI needs role assignments: Key Vault Secrets User, Storage Blob Data Contributor, and a contained SQL user from external provider with rights.
- Audience must match the token audience.