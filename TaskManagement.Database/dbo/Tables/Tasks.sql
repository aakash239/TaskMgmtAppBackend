CREATE TABLE [dbo].[Tasks]
(
    [TaskId] INT IDENTITY(1,1) NOT NULL,
    [BoardId] INT NOT NULL,
    [BoardColumnId] INT NOT NULL,

    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(2000) NULL,

    [Priority] TINYINT NOT NULL
        CONSTRAINT [DF_Tasks_Priority] DEFAULT (1),

    [DueDate] DATE NULL,

    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_Tasks_CreatedAt] DEFAULT (SYSUTCDATETIME()),

    [UpdatedAt] DATETIME2 NULL,

    [IsDeleted] BIT NOT NULL
        CONSTRAINT [DF_Tasks_IsDeleted] DEFAULT (0),

    [DeletedAt] DATETIME2 NULL,

    CONSTRAINT [PK_Tasks]
        PRIMARY KEY ([TaskId]),

    CONSTRAINT [FK_Tasks_Boards]
        FOREIGN KEY ([BoardId])
        REFERENCES [dbo].[Boards]([BoardId]), 
    
    CONSTRAINT [FK_Tasks_BoardColumns]
    FOREIGN KEY ([BoardColumnId])
    REFERENCES [dbo].[BoardColumns]([BoardColumnId]),
   

    CONSTRAINT [CK_Tasks_Priority]
        CHECK ([Priority] IN (0, 1, 2))
);
GO

CREATE INDEX [IX_Tasks_BoardId]
ON [dbo].[Tasks]([BoardId]);
GO

