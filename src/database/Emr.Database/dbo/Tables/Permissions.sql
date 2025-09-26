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

CREATE UNIQUE INDEX [IX_Permissions_Resource_Action_Scope] ON [dbo].[Permissions] ([Resource], [Action], [Scope]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_Permissions_IsDeleted] ON [dbo].[Permissions] ([IsDeleted]);