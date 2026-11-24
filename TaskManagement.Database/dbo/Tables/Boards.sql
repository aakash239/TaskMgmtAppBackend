CREATE TABLE [dbo].[Boards](
    [BoardId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [TemplateId] INT NOT NULL,
    [ProjectId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,

    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_Boards_CreatedAt]
            DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_Boards_UpdatedAt]
            DEFAULT (SYSUTCDATETIME()),
    [IsDeleted] BIT NOT NULL
        CONSTRAINT [DF_Boards_IsDeleted]
        DEFAULT (0),
    [DeletedAt] DATETIME2 Null,
    
    CONSTRAINT [PK_Boards]
        PRIMARY KEY ([BoardId]),

    CONSTRAINT [FK_Boards_Users]
        FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users]([UserId]),

    CONSTRAINT [FK_Boards_Projects]
        FOREIGN KEY([ProjectId])
            REFERENCES [dbo].[Projects](ProjectId),

    CONSTRAINT [FK_Boards_KanbanTemplates]
        FOREIGN KEY([TemplateId])
            REFERENCES [dbo].[KanbanTemplates]([TemplateId]),

    CONSTRAINT [UQ_Boards_ProjectId_UserId]
        UNIQUE([ProjectId], [UserId])
);

GO

CREATE INDEX [IDX_Board_ProjectId]
    ON [dbo].[Boards]([ProjectId]);

GO

CREATE INDEX [IDX_Boards_TemplateId]
    ON [dbo].[Boards]([TemplateId]);

GO

CREATE INDEX [IDX_Board_UserId]
    ON [dbo].[Boards]([UserId]);

GO
