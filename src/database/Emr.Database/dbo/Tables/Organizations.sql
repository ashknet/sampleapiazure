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

CREATE UNIQUE INDEX [IX_Organizations_TaxId] ON [dbo].[Organizations] ([TaxId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Organizations_Name] ON [dbo].[Organizations] ([Name]);
CREATE INDEX [IX_Organizations_IsDeleted] ON [dbo].[Organizations] ([IsDeleted]);