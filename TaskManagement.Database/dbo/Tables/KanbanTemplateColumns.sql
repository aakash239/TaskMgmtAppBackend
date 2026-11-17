CREATE TABLE [dbo].[KanbanTemplateColumns](
    [TemplateColumnId] INT IDENTITY(1,1) NOT NULL,
    [TemplateId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Position] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_KanbanTemplateColumns_CreatedAt]
            DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_KanbanTemplateColumns_UpdatedAt]
            DEFAULT (SYSUTCDATETIME()),
    
    CONSTRAINT [PK_KanbanTemplateColumns]
        PRIMARY KEY ([TemplateColumnId]),

    CONSTRAINT [FK_KanbanTemplateColumns_KanbanTemplates]
        FOREIGN KEY ([TemplateId])
            REFERENCES [dbo].[KanbanTemplates]([TemplateId])
            ON DELETE CASCADE,
    
    CONSTRAINT [UQ_KanbanTemplateColumns_Position]
        UNIQUE ([TemplateId], [Position]),
    
    CONSTRAINT [CK_KanbanTemplateColumns_Position]
        CHECK ([Position] >= 1)
);

GO