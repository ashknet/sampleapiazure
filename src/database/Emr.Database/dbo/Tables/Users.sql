CREATE TABLE [dbo].[Users]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ExternalId] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [MiddleName] NVARCHAR(100) NULL,
    [UserType] NVARCHAR(50) NOT NULL,
    [Phone] NVARCHAR(20) NULL,
    [DateOfBirth] DATE NULL,
    [NPI] NVARCHAR(10) NULL,
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

CREATE UNIQUE INDEX [IX_Users_ExternalId] ON [dbo].[Users] ([ExternalId]) WHERE [IsDeleted] = 0;
CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Users_IsDeleted] ON [dbo].[Users] ([IsDeleted]);