@echo off
REM Drop and recreate EMR Database
REM WARNING: This will delete all data!

echo EMR Database Drop and Recreate Script
echo =====================================
echo.
echo WARNING: This will DELETE ALL DATA in the database!
echo.

set /p confirm="Are you sure you want to drop and recreate the database? (yes/no): "
if /i not "%confirm%"=="yes" (
    echo Operation cancelled.
    pause
    exit /b 0
)

set SERVER=(localdb)\mssqllocaldb
set DATABASE=EmrDb_Dev

echo.
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

echo Dropping existing database...
sqlcmd -S %SERVER% -E -Q "USE master; IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'%DATABASE%') BEGIN ALTER DATABASE [%DATABASE%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%DATABASE%]; END"

echo.
echo Creating new database...
sqlcmd -S %SERVER% -E -Q "CREATE DATABASE [%DATABASE%]"

echo.
echo Creating tables...
sqlcmd -S %SERVER% -d %DATABASE% -E -i "dbo\Scripts\CreateAllTables.sql"

echo.
echo Running post-deployment script...
sqlcmd -S %SERVER% -d %DATABASE% -E -i "dbo\Scripts\PostDeployment.sql"

echo.
echo Database recreated successfully!
echo All tables have been created with the correct schema.
echo.

pause