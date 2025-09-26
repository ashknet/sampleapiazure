@echo off
REM EMR Database Deployment Script for Windows
REM This batch file creates the EMR database using sqlcmd

echo EMR Database Deployment Script
echo ==============================
echo.

set SERVER=(localdb)\mssqllocaldb
set DATABASE=EmrDb_Dev

echo Server: %SERVER%
echo Database: %DATABASE%
echo.

REM Check if sqlcmd is available
where sqlcmd >nul 2>nul
if errorlevel 1 (
    echo ERROR: sqlcmd not found. Please install SQL Server client tools.
    echo Download from: https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility
    pause
    exit /b 1
)

echo Creating database if it doesn't exist...
sqlcmd -S %SERVER% -E -Q "IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'%DATABASE%') CREATE DATABASE [%DATABASE%]"

echo.
echo Creating tables...
sqlcmd -S %SERVER% -d %DATABASE% -E -i "dbo\Scripts\CreateAllTables.sql"

echo.
echo Running post-deployment script...
sqlcmd -S %SERVER% -d %DATABASE% -E -i "dbo\Scripts\PostDeployment.sql"

echo.
echo Database deployment completed successfully!
echo.
echo Next steps:
echo 1. Update connection string in appsettings.Development.json
echo 2. Set "UseInMemoryDatabase": false
echo 3. Run the API project

pause