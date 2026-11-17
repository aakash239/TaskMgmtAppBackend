CREATE TABLE [dbo].[KanbanTemplates](
    [TemplateId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_KanbanTemplates_CreatedAt]
        DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_KanbanTemplates_UpdatedAt]
        DEFAULT (SYSUTCDATETIME()),
    
    CONSTRAINT [PK_KanbanTemplates]
        PRIMARY KEY ([TemplateId]),

    CONSTRAINT [UQ_KanbanTemplates_TemplateId_UserId]
        UNIQUE ([TemplateId], [UserId]),

    CONSTRAINT [FK_KanbanTemplate_Users]
        FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users]([UserId])
);

GO

CREATE INDEX [IDX_KanbanTemplates_UserId]
    ON [dbo].[KanbanTemplates]([UserId]);

GO