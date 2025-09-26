-- EMR Database Deployment Script
-- Run this script in SQL Server Management Studio or Azure Data Studio
-- 
-- Instructions:
-- 1. Connect to your SQL Server instance
-- 2. Change the database name below if needed
-- 3. Execute this script

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'EmrDb_Dev')
BEGIN
    CREATE DATABASE [EmrDb_Dev];
    PRINT 'Database EmrDb_Dev created.';
END
ELSE
BEGIN
    PRINT 'Database EmrDb_Dev already exists.';
END
GO

-- Switch to the database
USE [EmrDb_Dev];
GO

-- Include the create tables script
:r .\dbo\Scripts\CreateAllTables.sql
GO

-- Include the post-deployment script
:r .\dbo\Scripts\PostDeployment.sql
GO

PRINT '';
PRINT 'Database deployment completed successfully!';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Update the connection string in appsettings.Development.json';
PRINT '2. Run Entity Framework migrations from the API project:';
PRINT '   cd src/api/Emr.Api';
PRINT '   dotnet ef database update';