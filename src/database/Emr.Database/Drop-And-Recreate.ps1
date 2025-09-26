# Drop and recreate EMR Database
# WARNING: This will delete all data!

param(
    [Parameter(Mandatory=$false)]
    [string]$ServerInstance = "(localdb)\mssqllocaldb",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "EmrDb_Dev"
)

Write-Host "EMR Database Drop and Recreate Script" -ForegroundColor Red
Write-Host "=====================================" -ForegroundColor Red
Write-Host ""
Write-Host "WARNING: This will DELETE ALL DATA in the database!" -ForegroundColor Yellow
Write-Host ""
Write-Host "Server: $ServerInstance" -ForegroundColor Cyan
Write-Host "Database: $DatabaseName" -ForegroundColor Cyan
Write-Host ""

$confirmation = Read-Host "Are you sure you want to drop and recreate the database? (yes/no)"
if ($confirmation -ne 'yes') {
    Write-Host "Operation cancelled." -ForegroundColor Green
    exit
}

# Check if SqlServer module is installed
if (!(Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "SqlServer module not found. Installing..." -ForegroundColor Yellow
    Install-Module -Name SqlServer -Scope CurrentUser -Force -AllowClobber
}

Import-Module SqlServer

# Drop existing database
Write-Host "Dropping existing database..." -ForegroundColor Yellow
$dropScript = @"
USE master;
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$DatabaseName')
BEGIN
    ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$DatabaseName];
END
"@

try {
    Invoke-Sqlcmd -ServerInstance $ServerInstance -Query $dropScript -ErrorAction Stop
    Write-Host "Database dropped successfully." -ForegroundColor Green
}
catch {
    Write-Host "Error dropping database: $_" -ForegroundColor Red
}

# Create new database
Write-Host "Creating new database..." -ForegroundColor Green
$createDbScript = @"
CREATE DATABASE [$DatabaseName]
"@
Invoke-Sqlcmd -ServerInstance $ServerInstance -Query $createDbScript

# Execute the create tables script
Write-Host "Creating tables..." -ForegroundColor Green
$tablesScript = Get-Content -Path "$PSScriptRoot\dbo\Scripts\CreateAllTables.sql" -Raw
Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $tablesScript

# Execute the post-deployment script
Write-Host "Running post-deployment script..." -ForegroundColor Green
$postDeployScript = Get-Content -Path "$PSScriptRoot\dbo\Scripts\PostDeployment.sql" -Raw
Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $DatabaseName -Query $postDeployScript

Write-Host ""
Write-Host "Database recreated successfully!" -ForegroundColor Green
Write-Host "All tables have been created with the correct schema." -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run the API project: cd src/api/Emr.Api && dotnet run" -ForegroundColor White