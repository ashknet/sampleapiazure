-- Fix Payers table column names to match Entity Framework model
-- This script should be run if the Payers table has the old column names

-- Check if the old columns exist and rename them
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payers]') AND name = 'Phone')
BEGIN
    EXEC sp_rename 'dbo.Payers.Phone', 'ContactPhone', 'COLUMN';
    PRINT 'Renamed Payers.Phone to ContactPhone';
END
ELSE
BEGIN
    PRINT 'ContactPhone column already exists in Payers table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payers]') AND name = 'Email')
BEGIN
    EXEC sp_rename 'dbo.Payers.Email', 'ContactEmail', 'COLUMN';
    PRINT 'Renamed Payers.Email to ContactEmail';
END
ELSE
BEGIN
    PRINT 'ContactEmail column already exists in Payers table';
END

PRINT 'Payers table column names fixed successfully!';