# Running the EMR API

## Windows
When running from Visual Studio or command line in Windows, ensure the environment is set to Development:

```powershell
# From PowerShell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run

# Or from Command Prompt
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

## Linux/Mac
```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

## Visual Studio
The launchSettings.json already sets ASPNETCORE_ENVIRONMENT to Development for all profiles.

## Troubleshooting

### If you see SQL Server connection errors:
1. Ensure ASPNETCORE_ENVIRONMENT is set to "Development"
2. Check that appsettings.Development.json exists
3. Verify "UseInMemoryDatabase": true in appsettings.Development.json

### Query Filter Warnings
The warnings about query filters are informational and won't prevent the app from running. They indicate that soft-delete filters are applied globally to all entities.

### To run with SQL Server instead:
1. Set "UseInMemoryDatabase": false in appsettings.Development.json
2. Update the connection string
3. Run migrations: `dotnet ef database update`