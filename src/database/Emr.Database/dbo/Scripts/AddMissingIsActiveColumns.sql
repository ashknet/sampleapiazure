-- Migration script to add IsActive columns to tables that are missing them
-- This script should be run if the tables already exist without the IsActive columns

-- Add IsActive to Locations table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[Locations]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;
    
    PRINT 'Added IsActive column to Locations table with default value of 1 (true)';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in Locations table';
END
GO

-- Add IsActive to Departments table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[Departments]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;
    
    PRINT 'Added IsActive column to Departments table with default value of 1 (true)';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in Departments table';
END
GO

-- Add IsActive to PatientIdentifiers table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PatientIdentifiers]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[PatientIdentifiers]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;
    
    PRINT 'Added IsActive column to PatientIdentifiers table with default value of 1 (true)';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in PatientIdentifiers table';
END
GO

-- Add IsActive to PatientContacts table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PatientContacts]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[PatientContacts]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;
    
    PRINT 'Added IsActive column to PatientContacts table with default value of 1 (true)';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in PatientContacts table';
END
GO

-- Add IsActive to PatientAddresses table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PatientAddresses]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[PatientAddresses]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;
    
    PRINT 'Added IsActive column to PatientAddresses table with default value of 1 (true)';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in PatientAddresses table';
END
GO

-- Add IsActive to Coverages table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Coverages]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[Coverages]
    ADD [IsActive] BIT NOT NULL DEFAULT 1;
    
    PRINT 'Added IsActive column to Coverages table with default value of 1 (true)';
END
ELSE
BEGIN
    PRINT 'IsActive column already exists in Coverages table';
END
GO