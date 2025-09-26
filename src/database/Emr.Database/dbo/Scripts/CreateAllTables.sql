-- Create all EMR database tables
-- This script creates all tables in the correct order to respect foreign key constraints

-- Organizations table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Organizations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Organizations]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [TaxId] NVARCHAR(20) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [NpiNumber] NVARCHAR(10) NULL,
    [ContactEmail] NVARCHAR(255) NULL,
    [ContactPhone] NVARCHAR(20) NULL,
    [Website] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- Locations table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Locations]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Phone] NVARCHAR(20) NULL,
    [Fax] NVARCHAR(20) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [Street1] NVARCHAR(200) NOT NULL,
    [Street2] NVARCHAR(200) NULL,
    [City] NVARCHAR(100) NOT NULL,
    [State] NVARCHAR(2) NOT NULL,
    [PostalCode] NVARCHAR(10) NOT NULL,
    [Country] NVARCHAR(3) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Locations_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id])
);
END
GO

-- Departments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Departments]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [LocationId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Specialty] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Departments_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id]),
    CONSTRAINT [FK_Departments_Locations] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id])
);
END
GO

-- Users table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Users]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ExternalId] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [MiddleName] NVARCHAR(100) NULL,
    [UserType] NVARCHAR(50) NOT NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    [DateOfBirth] DATE NULL,
    [NpiNumber] NVARCHAR(10) NULL,
    [LicenseNumber] NVARCHAR(50) NULL,
    [Specialty] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastLoginAt] DATETIME2 NULL,
    [EmailVerified] BIT NOT NULL DEFAULT 0,
    [EmailVerifiedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- Roles table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Roles]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [DisplayName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsSystem] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- Permissions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Permissions]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Resource] NVARCHAR(50) NOT NULL,
    [Action] NVARCHAR(50) NOT NULL,
    [Scope] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- UserRoles table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserRoles]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    [ExpiresAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [FK_UserRoles_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id])
);
END
GO

-- RolePermissions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RolePermissions]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [PermissionId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions] ([Id])
);
END
GO

-- UserOrganizationAssignments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOrganizationAssignments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserOrganizationAssignments]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId] UNIQUEIDENTIFIER NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_UserOrganizationAssignments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserOrganizationAssignments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_UserOrganizationAssignments_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id]),
    CONSTRAINT [FK_UserOrganizationAssignments_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id])
);
END
GO

-- Patients table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Patients]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Patients]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [MedicalRecordNumber] NVARCHAR(50) NOT NULL,
    [SocialSecurityNumber] NVARCHAR(20) NULL,
    [Gender] NVARCHAR(10) NOT NULL,
    [PreferredLanguage] NVARCHAR(10) NULL,
    [Race] NVARCHAR(50) NULL,
    [Ethnicity] NVARCHAR(50) NULL,
    [MaritalStatus] NVARCHAR(20) NULL,
    [Religion] NVARCHAR(50) NULL,
    [IsDeceased] BIT NOT NULL DEFAULT 0,
    [DeceasedDate] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Patients_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
END
GO

-- PatientIdentifiers table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PatientIdentifiers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PatientIdentifiers]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [Value] NVARCHAR(100) NOT NULL,
    [Issuer] NVARCHAR(200) NULL,
    [StartDate] DATE NULL,
    [EndDate] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_PatientIdentifiers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PatientIdentifiers_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id])
);
END
GO

-- PatientContacts table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PatientContacts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PatientContacts]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Relationship] NVARCHAR(50) NOT NULL,
    [Phone] NVARCHAR(20) NULL,
    [Email] NVARCHAR(255) NULL,
    [IsEmergencyContact] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_PatientContacts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PatientContacts_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id])
);
END
GO

-- PatientAddresses table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PatientAddresses]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PatientAddresses]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    [Street1] NVARCHAR(200) NOT NULL,
    [Street2] NVARCHAR(200) NULL,
    [City] NVARCHAR(100) NOT NULL,
    [State] NVARCHAR(2) NOT NULL,
    [PostalCode] NVARCHAR(10) NOT NULL,
    [Country] NVARCHAR(3) NOT NULL,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_PatientAddresses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PatientAddresses_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id])
);
END
GO

-- Payers table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Payers]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [PayerId] NVARCHAR(50) NULL,
    [ContactPhone] NVARCHAR(20) NULL,
    [ContactEmail] NVARCHAR(255) NULL,
    [Website] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Payers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- Plans table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Plans]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Plans]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PayerId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [GroupNumber] NVARCHAR(50) NULL,
    [RequiresReferral] BIT NOT NULL DEFAULT 0,
    [RequiresPreAuthorization] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Plans] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Plans_Payers] FOREIGN KEY ([PayerId]) REFERENCES [dbo].[Payers] ([Id])
);
END
GO

-- Coverages table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Coverages]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Coverages]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [PlanId] UNIQUEIDENTIFIER NOT NULL,
    [MemberId] NVARCHAR(50) NOT NULL,
    [GroupNumber] NVARCHAR(50) NULL,
    [Priority] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Coverages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Coverages_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id]),
    CONSTRAINT [FK_Coverages_Plans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[Plans] ([Id])
);
END
GO

-- Consents table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Consents]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Consents]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [ConsentDate] DATETIME2 NOT NULL,
    [ExpirationDate] DATETIME2 NULL,
    [PurposeOfUse] NVARCHAR(200) NULL,
    [ConsentingParty] NVARCHAR(255) NULL,
    [RevokedBy] NVARCHAR(255) NULL,
    [RevokedDate] DATETIME2 NULL,
    [ScopeJson] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Consents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Consents_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id]),
    CONSTRAINT [FK_Consents_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id])
);
END
GO

-- ConsentEvents table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsentEvents]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ConsentEvents]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ConsentId] UNIQUEIDENTIFIER NOT NULL,
    [EventType] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [PerformedBy] NVARCHAR(255) NULL,
    [OccurredAt] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_ConsentEvents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConsentEvents_Consents] FOREIGN KEY ([ConsentId]) REFERENCES [dbo].[Consents] ([Id])
);
END
GO

-- ShareTokens table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShareTokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ShareTokens]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Code] NVARCHAR(10) NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [RequestedScopes] NVARCHAR(MAX) NULL,
    [AuthorizedScopes] NVARCHAR(MAX) NULL,
    [AuthorizedAt] DATETIME2 NULL,
    [AuthorizedBy] NVARCHAR(255) NULL,
    [LastAccessedAt] DATETIME2 NULL,
    [AccessCount] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_ShareTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ShareTokens_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id]),
    CONSTRAINT [FK_ShareTokens_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id])
);
END
GO

-- ConnectionLinks table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectionLinks]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ConnectionLinks]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ShareTokenId] UNIQUEIDENTIFIER NOT NULL,
    [AccessToken] NVARCHAR(500) NOT NULL,
    [IssuedAt] DATETIME2 NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [IsRevoked] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_ConnectionLinks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConnectionLinks_ShareTokens] FOREIGN KEY ([ShareTokenId]) REFERENCES [dbo].[ShareTokens] ([Id])
);
END
GO

-- Documents table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Documents]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Documents]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NULL,
    [EncounterId] UNIQUEIDENTIFIER NULL,
    [UploadedByUserId] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(500) NOT NULL,
    [DocumentType] NVARCHAR(50) NOT NULL,
    [Category] NVARCHAR(100) NULL,
    [FileName] NVARCHAR(500) NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [FileSizeBytes] BIGINT NOT NULL,
    [BlobStorageUrl] NVARCHAR(MAX) NOT NULL,
    [Checksum] NVARCHAR(100) NULL,
    [DocumentDate] DATETIME2 NOT NULL,
    [IsConfidential] BIT NOT NULL DEFAULT 0,
    [ConfidentialityCode] NVARCHAR(50) NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [FhirDocumentReferenceId] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Documents_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id]),
    CONSTRAINT [FK_Documents_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id]),
    CONSTRAINT [FK_Documents_Users] FOREIGN KEY ([UploadedByUserId]) REFERENCES [dbo].[Users] ([Id])
);
END
GO

-- DocumentShares table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DocumentShares]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DocumentShares]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [DocumentId] UNIQUEIDENTIFIER NOT NULL,
    [SharedWithUserId] UNIQUEIDENTIFIER NOT NULL,
    [SharedByUserId] UNIQUEIDENTIFIER NOT NULL,
    [SharedAt] DATETIME2 NOT NULL,
    [ExpiresAt] DATETIME2 NULL,
    [Purpose] NVARCHAR(500) NULL,
    [CanDownload] BIT NOT NULL DEFAULT 1,
    [CanPrint] BIT NOT NULL DEFAULT 1,
    [LastAccessedAt] DATETIME2 NULL,
    [AccessCount] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_DocumentShares] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DocumentShares_Documents] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Documents] ([Id]),
    CONSTRAINT [FK_DocumentShares_Users_SharedWith] FOREIGN KEY ([SharedWithUserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_DocumentShares_Users_SharedBy] FOREIGN KEY ([SharedByUserId]) REFERENCES [dbo].[Users] ([Id])
);
END
GO

-- Appointments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Appointments]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [LocationId] UNIQUEIDENTIFIER NULL,
    [DepartmentId] UNIQUEIDENTIFIER NULL,
    [ProviderId] UNIQUEIDENTIFIER NULL,
    [AppointmentType] NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [StartDateTime] DATETIME2 NOT NULL,
    [EndDateTime] DATETIME2 NOT NULL,
    [Reason] NVARCHAR(500) NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [IsVirtual] BIT NOT NULL DEFAULT 0,
    [VirtualMeetingUrl] NVARCHAR(500) NULL,
    [CheckInTime] DATETIME2 NULL,
    [CheckOutTime] DATETIME2 NULL,
    [CancellationReason] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Appointments_Patients] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients] ([Id]),
    CONSTRAINT [FK_Appointments_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id]),
    CONSTRAINT [FK_Appointments_Locations] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]),
    CONSTRAINT [FK_Appointments_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]),
    CONSTRAINT [FK_Appointments_Users] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Users] ([Id])
);
END
GO

-- AuditLogs table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AuditLogs]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] NVARCHAR(255) NOT NULL,
    [UserName] NVARCHAR(255) NOT NULL,
    [UserRole] NVARCHAR(100) NOT NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [Resource] NVARCHAR(100) NOT NULL,
    [ResourceId] NVARCHAR(255) NOT NULL,
    [PatientId] NVARCHAR(255) NULL,
    [OrganizationId] NVARCHAR(255) NULL,
    [Timestamp] DATETIME2 NOT NULL,
    [IpAddress] NVARCHAR(50) NOT NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [PurposeOfUse] NVARCHAR(200) NULL,
    [ConsentId] NVARCHAR(255) NULL,
    [Success] BIT NOT NULL DEFAULT 1,
    [FailureReason] NVARCHAR(500) NULL,
    [AdditionalData] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- IntegrationEndpoints table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IntegrationEndpoints]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[IntegrationEndpoints]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [Direction] NVARCHAR(50) NOT NULL,
    [EndpointUrl] NVARCHAR(500) NOT NULL,
    [AuthType] NVARCHAR(50) NULL,
    [ConfigurationJson] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastSuccessfulConnection] DATETIME2 NULL,
    [LastError] NVARCHAR(500) NULL,
    [LastErrorAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_IntegrationEndpoints] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_IntegrationEndpoints_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([Id])
);
END
GO

-- NotificationPreferences table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NotificationPreferences]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NotificationPreferences]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Channel] NVARCHAR(50) NOT NULL,
    [NotificationType] NVARCHAR(50) NOT NULL,
    [IsEnabled] BIT NOT NULL DEFAULT 1,
    [Frequency] NVARCHAR(50) NULL,
    [AdditionalSettings] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(255) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(255) NULL,
    CONSTRAINT [PK_NotificationPreferences] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NotificationPreferences_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
END
GO

-- Create Indexes
CREATE UNIQUE INDEX [IX_Organizations_TaxId] ON [dbo].[Organizations] ([TaxId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Organizations_Name] ON [dbo].[Organizations] ([Name]);
CREATE INDEX [IX_Organizations_IsDeleted] ON [dbo].[Organizations] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Locations_OrganizationId_Code] ON [dbo].[Locations] ([OrganizationId], [Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Locations_IsDeleted] ON [dbo].[Locations] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Departments_OrganizationId_Code] ON [dbo].[Departments] ([OrganizationId], [Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Departments_IsDeleted] ON [dbo].[Departments] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Users_ExternalId] ON [dbo].[Users] ([ExternalId]) WHERE [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Users_IsDeleted] ON [dbo].[Users] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Roles_Name] ON [dbo].[Roles] ([Name]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Roles_IsDeleted] ON [dbo].[Roles] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Permissions_Resource_Action_Scope] ON [dbo].[Permissions] ([Resource], [Action], [Scope]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Permissions_IsDeleted] ON [dbo].[Permissions] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_UserRoles_UserId_RoleId_OrganizationId] ON [dbo].[UserRoles] ([UserId], [RoleId], [OrganizationId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_UserRoles_IsDeleted] ON [dbo].[UserRoles] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_RolePermissions_RoleId_PermissionId] ON [dbo].[RolePermissions] ([RoleId], [PermissionId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_RolePermissions_IsDeleted] ON [dbo].[RolePermissions] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Patients_MedicalRecordNumber] ON [dbo].[Patients] ([MedicalRecordNumber]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Patients_UserId] ON [dbo].[Patients] ([UserId]);
CREATE INDEX [IX_Patients_IsDeleted] ON [dbo].[Patients] ([IsDeleted]);

CREATE INDEX [IX_PatientIdentifiers_PatientId] ON [dbo].[PatientIdentifiers] ([PatientId]);
CREATE INDEX [IX_PatientIdentifiers_Type_Value] ON [dbo].[PatientIdentifiers] ([Type], [Value]);
CREATE INDEX [IX_PatientIdentifiers_IsDeleted] ON [dbo].[PatientIdentifiers] ([IsDeleted]);

CREATE INDEX [IX_PatientContacts_PatientId] ON [dbo].[PatientContacts] ([PatientId]);
CREATE INDEX [IX_PatientContacts_IsDeleted] ON [dbo].[PatientContacts] ([IsDeleted]);

CREATE INDEX [IX_PatientAddresses_PatientId] ON [dbo].[PatientAddresses] ([PatientId]);
CREATE INDEX [IX_PatientAddresses_IsDeleted] ON [dbo].[PatientAddresses] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Payers_Code] ON [dbo].[Payers] ([Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Payers_IsDeleted] ON [dbo].[Payers] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_Plans_PayerId_Code] ON [dbo].[Plans] ([PayerId], [Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Plans_IsDeleted] ON [dbo].[Plans] ([IsDeleted]);

CREATE INDEX [IX_Coverages_PatientId] ON [dbo].[Coverages] ([PatientId]);
CREATE INDEX [IX_Coverages_PlanId] ON [dbo].[Coverages] ([PlanId]);
CREATE INDEX [IX_Coverages_MemberId] ON [dbo].[Coverages] ([MemberId]);
CREATE INDEX [IX_Coverages_IsDeleted] ON [dbo].[Coverages] ([IsDeleted]);

CREATE INDEX [IX_Consents_PatientId] ON [dbo].[Consents] ([PatientId]);
CREATE INDEX [IX_Consents_Status] ON [dbo].[Consents] ([Status]);
CREATE INDEX [IX_Consents_IsDeleted] ON [dbo].[Consents] ([IsDeleted]);

CREATE INDEX [IX_ConsentEvents_ConsentId] ON [dbo].[ConsentEvents] ([ConsentId]);
CREATE INDEX [IX_ConsentEvents_IsDeleted] ON [dbo].[ConsentEvents] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_ShareTokens_Code] ON [dbo].[ShareTokens] ([Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_ShareTokens_PatientId] ON [dbo].[ShareTokens] ([PatientId]);
CREATE INDEX [IX_ShareTokens_OrganizationId] ON [dbo].[ShareTokens] ([OrganizationId]);
CREATE INDEX [IX_ShareTokens_IsDeleted] ON [dbo].[ShareTokens] ([IsDeleted]);

CREATE INDEX [IX_ConnectionLinks_ShareTokenId] ON [dbo].[ConnectionLinks] ([ShareTokenId]);
CREATE INDEX [IX_ConnectionLinks_IsDeleted] ON [dbo].[ConnectionLinks] ([IsDeleted]);

CREATE INDEX [IX_Documents_PatientId] ON [dbo].[Documents] ([PatientId]);
CREATE INDEX [IX_Documents_OrganizationId] ON [dbo].[Documents] ([OrganizationId]);
CREATE INDEX [IX_Documents_DocumentDate] ON [dbo].[Documents] ([DocumentDate]);
CREATE INDEX [IX_Documents_IsDeleted] ON [dbo].[Documents] ([IsDeleted]);

CREATE INDEX [IX_DocumentShares_DocumentId] ON [dbo].[DocumentShares] ([DocumentId]);
CREATE INDEX [IX_DocumentShares_SharedWithUserId] ON [dbo].[DocumentShares] ([SharedWithUserId]);
CREATE INDEX [IX_DocumentShares_IsDeleted] ON [dbo].[DocumentShares] ([IsDeleted]);

CREATE INDEX [IX_Appointments_PatientId] ON [dbo].[Appointments] ([PatientId]);
CREATE INDEX [IX_Appointments_OrganizationId] ON [dbo].[Appointments] ([OrganizationId]);
CREATE INDEX [IX_Appointments_ProviderId] ON [dbo].[Appointments] ([ProviderId]);
CREATE INDEX [IX_Appointments_StartDateTime] ON [dbo].[Appointments] ([StartDateTime]);
CREATE INDEX [IX_Appointments_Status] ON [dbo].[Appointments] ([Status]);
CREATE INDEX [IX_Appointments_IsDeleted] ON [dbo].[Appointments] ([IsDeleted]);

CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
CREATE INDEX [IX_AuditLogs_Resource_ResourceId] ON [dbo].[AuditLogs] ([Resource], [ResourceId]);
CREATE INDEX [IX_AuditLogs_Timestamp] ON [dbo].[AuditLogs] ([Timestamp]);
CREATE INDEX [IX_AuditLogs_IsDeleted] ON [dbo].[AuditLogs] ([IsDeleted]);

CREATE INDEX [IX_IntegrationEndpoints_OrganizationId] ON [dbo].[IntegrationEndpoints] ([OrganizationId]);
CREATE INDEX [IX_IntegrationEndpoints_IsDeleted] ON [dbo].[IntegrationEndpoints] ([IsDeleted]);

CREATE UNIQUE INDEX [IX_NotificationPreferences_UserId_Channel_NotificationType] ON [dbo].[NotificationPreferences] ([UserId], [Channel], [NotificationType]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_NotificationPreferences_IsDeleted] ON [dbo].[NotificationPreferences] ([IsDeleted]);
GO