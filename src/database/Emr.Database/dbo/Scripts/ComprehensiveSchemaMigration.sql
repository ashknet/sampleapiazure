-- Comprehensive Schema Migration Script
-- This script updates existing databases to match the Entity Framework models

-- =============================================
-- 1. PAYERS TABLE - Fix column name mismatches
-- =============================================
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payers]') AND name = 'Phone')
BEGIN
    EXEC sp_rename 'dbo.Payers.Phone', 'ContactPhone', 'COLUMN';
    PRINT 'Renamed Payers.Phone to ContactPhone';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payers]') AND name = 'Email')
BEGIN
    EXEC sp_rename 'dbo.Payers.Email', 'ContactEmail', 'COLUMN';
    PRINT 'Renamed Payers.Email to ContactEmail';
END

-- =============================================
-- 2. ORGANIZATIONS TABLE - Add missing IsActive configuration
-- =============================================
-- IsActive column already exists, just ensure it has proper default

-- =============================================
-- 3. USERS TABLE - Fix column name mismatches
-- =============================================
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Phone')
BEGIN
    EXEC sp_rename 'dbo.Users.Phone', 'PhoneNumber', 'COLUMN';
    PRINT 'Renamed Users.Phone to PhoneNumber';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'NPI')
BEGIN
    EXEC sp_rename 'dbo.Users.NPI', 'NpiNumber', 'COLUMN';
    PRINT 'Renamed Users.NPI to NpiNumber';
END

-- =============================================
-- 4. PATIENTS TABLE - Fix column name mismatches and add missing columns
-- =============================================
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Patients]') AND name = 'MRN')
BEGIN
    EXEC sp_rename 'dbo.Patients.MRN', 'MedicalRecordNumber', 'COLUMN';
    PRINT 'Renamed Patients.MRN to MedicalRecordNumber';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Patients]') AND name = 'SocialSecurityNumber')
BEGIN
    ALTER TABLE [dbo].[Patients] ADD [SocialSecurityNumber] NVARCHAR(20) NULL;
    PRINT 'Added SocialSecurityNumber column to Patients table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Patients]') AND name = 'IsDeceased')
BEGIN
    ALTER TABLE [dbo].[Patients] ADD [IsDeceased] BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsDeceased column to Patients table';
END

-- =============================================
-- 5. PLANS TABLE - Add missing columns
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Plans]') AND name = 'RequiresReferral')
BEGIN
    ALTER TABLE [dbo].[Plans] ADD [RequiresReferral] BIT NOT NULL DEFAULT 0;
    PRINT 'Added RequiresReferral column to Plans table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Plans]') AND name = 'RequiresAuthorization')
BEGIN
    EXEC sp_rename 'dbo.Plans.RequiresAuthorization', 'RequiresPreAuthorization', 'COLUMN';
    PRINT 'Renamed Plans.RequiresAuthorization to RequiresPreAuthorization';
END

-- =============================================
-- 6. USERORGANIZATIONASSIGNMENTS TABLE - Add missing column
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserOrganizationAssignments]') AND name = 'DepartmentId')
BEGIN
    ALTER TABLE [dbo].[UserOrganizationAssignments] ADD [DepartmentId] UNIQUEIDENTIFIER NULL;
    PRINT 'Added DepartmentId column to UserOrganizationAssignments table';
END

-- =============================================
-- 7. USERROLES TABLE - Add missing column
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND name = 'ExpiresAt')
BEGIN
    ALTER TABLE [dbo].[UserRoles] ADD [ExpiresAt] DATETIME2 NULL;
    PRINT 'Added ExpiresAt column to UserRoles table';
END

-- =============================================
-- 8. APPOINTMENTS TABLE - Major restructure
-- =============================================
-- Drop and recreate indexes that reference old column names
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_StartTime')
BEGIN
    DROP INDEX [IX_Appointments_StartTime] ON [dbo].[Appointments];
END

-- Rename columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'Type')
BEGIN
    EXEC sp_rename 'dbo.Appointments.Type', 'AppointmentType', 'COLUMN';
    PRINT 'Renamed Appointments.Type to AppointmentType';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'StartTime')
BEGIN
    EXEC sp_rename 'dbo.Appointments.StartTime', 'StartDateTime', 'COLUMN';
    PRINT 'Renamed Appointments.StartTime to StartDateTime';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'EndTime')
BEGIN
    EXEC sp_rename 'dbo.Appointments.EndTime', 'EndDateTime', 'COLUMN';
    PRINT 'Renamed Appointments.EndTime to EndDateTime';
END

-- Add missing columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'IsVirtual')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD [IsVirtual] BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsVirtual column to Appointments table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'VirtualMeetingUrl')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD [VirtualMeetingUrl] NVARCHAR(500) NULL;
    PRINT 'Added VirtualMeetingUrl column to Appointments table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'CheckInTime')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD [CheckInTime] DATETIME2 NULL;
    PRINT 'Added CheckInTime column to Appointments table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'CheckOutTime')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD [CheckOutTime] DATETIME2 NULL;
    PRINT 'Added CheckOutTime column to Appointments table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'CancellationReason')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD [CancellationReason] NVARCHAR(500) NULL;
    PRINT 'Added CancellationReason column to Appointments table';
END

-- Make LocationId nullable
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'LocationId' AND is_nullable = 0)
BEGIN
    ALTER TABLE [dbo].[Appointments] ALTER COLUMN [LocationId] UNIQUEIDENTIFIER NULL;
    PRINT 'Made LocationId nullable in Appointments table';
END

-- =============================================
-- 9. AUDITLOGS TABLE - Major restructure
-- =============================================
-- Add missing columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'UserName')
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ADD [UserName] NVARCHAR(255) NOT NULL DEFAULT '';
    PRINT 'Added UserName column to AuditLogs table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'UserRole')
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ADD [UserRole] NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT 'Added UserRole column to AuditLogs table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'Success')
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ADD [Success] BIT NOT NULL DEFAULT 1;
    PRINT 'Added Success column to AuditLogs table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'FailureReason')
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ADD [FailureReason] NVARCHAR(500) NULL;
    PRINT 'Added FailureReason column to AuditLogs table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'AdditionalData')
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ADD [AdditionalData] NVARCHAR(MAX) NULL;
    PRINT 'Added AdditionalData column to AuditLogs table';
END

-- Rename columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'EntityType')
BEGIN
    EXEC sp_rename 'dbo.AuditLogs.EntityType', 'Resource', 'COLUMN';
    PRINT 'Renamed AuditLogs.EntityType to Resource';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'EntityId')
BEGIN
    EXEC sp_rename 'dbo.AuditLogs.EntityId', 'ResourceId', 'COLUMN';
    PRINT 'Renamed AuditLogs.EntityId to ResourceId';
END

-- Make IpAddress required
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'IpAddress' AND is_nullable = 1)
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [IpAddress] NVARCHAR(50) NOT NULL;
    PRINT 'Made IpAddress required in AuditLogs table';
END

PRINT 'Comprehensive schema migration completed successfully!';