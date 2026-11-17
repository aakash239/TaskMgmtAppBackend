CREATE TABLE [dbo].[Boards](
    [BoardId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [TemplateId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,

    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_Boards_CreatedAt]
            DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_Boards_UpdatedAt]
            DEFAULT (SYSUTCDATETIME()),
    
    CONSTRAINT [PK_Boards]
        PRIMARY KEY ([BoardId]),

    CONSTRAINT [FK_Boards_Users]
        FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users]([UserId]),

    CONSTRAINT [FK_Boards_KanbanTemplates]
        FOREIGN KEY ([TemplateId], [UserId])
            REFERENCES [dbo].[KanbanTemplates]([TemplateId], [UserId]),
);

GO

CREATE INDEX [IDX_Board_UserId]
    ON [dbo].[Boards]([UserId]);

GO

CREATE INDEX [IDX_Boards_TemplateId_UserId]
    ON [dbo].[Boards]([TemplateId], [UserId]);

GO
