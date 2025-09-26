CREATE TABLE [dbo].[Departments]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [LocationId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Specialty] NVARCHAR(100) NULL,
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

CREATE UNIQUE INDEX [IX_Departments_OrganizationId_Code] ON [dbo].[Departments] ([OrganizationId], [Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Departments_IsDeleted] ON [dbo].[Departments] ([IsDeleted]);