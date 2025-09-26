-- Drop and recreate the EMR database
-- WARNING: This will delete all data!

USE master;
GO

-- Drop existing database if it exists
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'EmrDb_Dev')
BEGIN
    ALTER DATABASE [EmrDb_Dev] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [EmrDb_Dev];
    PRINT 'Database EmrDb_Dev dropped.';
END
GO

-- Create new database
CREATE DATABASE [EmrDb_Dev];
PRINT 'Database EmrDb_Dev created.';
GO

-- Switch to the database
USE [EmrDb_Dev];
GO

-- Run the create tables script
:r .\dbo\Scripts\CreateAllTables.sql
GO

-- Run the post-deployment script
:r .\dbo\Scripts\PostDeployment.sql
GO

PRINT '';
PRINT 'Database recreated successfully!';
PRINT 'All tables have been created with the correct schema.';