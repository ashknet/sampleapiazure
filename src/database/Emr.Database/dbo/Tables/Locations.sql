CREATE TABLE [dbo].[Locations]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Phone] NVARCHAR(20) NULL,
    [Fax] NVARCHAR(20) NULL,
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

CREATE UNIQUE INDEX [IX_Locations_OrganizationId_Code] ON [dbo].[Locations] ([OrganizationId], [Code]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Locations_IsDeleted] ON [dbo].[Locations] ([IsDeleted]);