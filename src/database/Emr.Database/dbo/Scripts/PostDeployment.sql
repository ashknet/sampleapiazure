/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/

-- Seed default roles
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Patient')
BEGIN
    INSERT INTO [dbo].[Roles] ([Id], [Name], [DisplayName], [Description], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), 'Patient', 'Patient', 'Patient role with access to personal health records', 1, GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Clinician')
BEGIN
    INSERT INTO [dbo].[Roles] ([Id], [Name], [DisplayName], [Description], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), 'Clinician', 'Clinician', 'Clinical staff with patient care access', 1, GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Registrar')
BEGIN
    INSERT INTO [dbo].[Roles] ([Id], [Name], [DisplayName], [Description], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), 'Registrar', 'Registrar', 'Registration staff with patient intake access', 1, GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'HIM')
BEGIN
    INSERT INTO [dbo].[Roles] ([Id], [Name], [DisplayName], [Description], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), 'HIM', 'Health Information Management', 'HIM staff with records management access', 1, GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'OrgAdmin')
BEGIN
    INSERT INTO [dbo].[Roles] ([Id], [Name], [DisplayName], [Description], [IsSystem], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), 'OrgAdmin', 'Organization Administrator', 'Organization administrator with full access', 1, GETUTCDATE(), 'system');
END

-- Seed default permissions
DECLARE @PatientResource NVARCHAR(50) = 'Patient';
DECLARE @DocumentResource NVARCHAR(50) = 'Document';
DECLARE @ConsentResource NVARCHAR(50) = 'Consent';
DECLARE @AppointmentResource NVARCHAR(50) = 'Appointment';
DECLARE @OrganizationResource NVARCHAR(50) = 'Organization';

-- Patient permissions
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @PatientResource AND [Action] = 'Read' AND [Scope] = 'Own')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @PatientResource, 'Read', 'Own', 'Read own patient record', GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @PatientResource AND [Action] = 'Update' AND [Scope] = 'Own')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @PatientResource, 'Update', 'Own', 'Update own patient demographics', GETUTCDATE(), 'system');
END

-- Document permissions
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @DocumentResource AND [Action] = 'Read' AND [Scope] = 'Own')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @DocumentResource, 'Read', 'Own', 'Read own documents', GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @DocumentResource AND [Action] = 'Read' AND [Scope] = 'Organization')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @DocumentResource, 'Read', 'Organization', 'Read organization documents', GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @DocumentResource AND [Action] = 'Create' AND [Scope] = 'Organization')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @DocumentResource, 'Create', 'Organization', 'Create documents for organization patients', GETUTCDATE(), 'system');
END

-- Consent permissions
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @ConsentResource AND [Action] = 'Read' AND [Scope] = 'Own')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @ConsentResource, 'Read', 'Own', 'Read own consents', GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @ConsentResource AND [Action] = 'Create' AND [Scope] = 'Own')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @ConsentResource, 'Create', 'Own', 'Create own consents', GETUTCDATE(), 'system');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Resource] = @ConsentResource AND [Action] = 'Revoke' AND [Scope] = 'Own')
BEGIN
    INSERT INTO [dbo].[Permissions] ([Id], [Resource], [Action], [Scope], [Description], [CreatedAt], [CreatedBy])
    VALUES (NEWID(), @ConsentResource, 'Revoke', 'Own', 'Revoke own consents', GETUTCDATE(), 'system');
END

-- Print completion message
PRINT 'Post-deployment script completed successfully.';