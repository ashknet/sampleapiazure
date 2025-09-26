# Deploy EMR Database Script
# This script creates the EMR database and all its objects

param(
    [Parameter(Mandatory=$false)]
    [string]$ServerInstance = "(localdb)\mssqllocaldb",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "EmrDb_Dev",
    
    [Parameter(Mandatory=$false)]
    [switch]$DropExisting = $false
)

Write-Host "EMR Database Deployment Script" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green
Write-Host ""
Write-Host "Server: $ServerInstance" -ForegroundColor Cyan
Write-Host "Database: $DatabaseName" -ForegroundColor Cyan
Write-Host ""

# Check if SqlServer module is installed
if (!(Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "SqlServer module not found. Installing..." -ForegroundColor Yellow
    Install-Module -Name SqlServer -Scope CurrentUser -Force -AllowClobber
}

Import-Module SqlServer

# Drop existing database if requested
if ($DropExisting) {
    Write-Host "Dropping existing database if it exists..." -ForegroundColor Yellow
    $dropScript = @"
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$DatabaseName')
BEGIN
    ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [$DatabaseName]
END
"@
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Query $dropScript -ErrorAction SilentlyContinue
}

# Create database
Write-Host "Creating database..." -ForegroundColor Green
$createDbScript = @"
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$DatabaseName')
BEGIN
    CREATE DATABASE [$DatabaseName]
END
"@
Invoke-Sqlcmd -ServerInstance $ServerInstance -Query $createDbScript

# Switch to the new database
Write-Host "Switching to database: $DatabaseName" -ForegroundColor Green

# Execute the create tables script
Write-Host "Creating tables..." -ForegroundColor Green
$tablesScript = Get-Content -Path "$PSScriptRoot\dbo\Scripts\CreateAllTables.sql" -Raw
Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $tablesScript

# Execute the post-deployment script
Write-Host "Running post-deployment script..." -ForegroundColor Green
$postDeployScript = Get-Content -Path "$PSScriptRoot\dbo\Scripts\PostDeployment.sql" -Raw
Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $postDeployScript

Write-Host ""
Write-Host "Database deployment completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "To run migrations from Entity Framework Core:" -ForegroundColor Yellow
Write-Host "  cd src/api/Emr.Api" -ForegroundColor White
Write-Host "  dotnet ef database update" -ForegroundColor White