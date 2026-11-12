CREATE TABLE [dbo].[BoardColumns](
    [BoardColumnId] INT IDENTITY(1,1) NOT NULL,
    [BoardId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Position] INT NULL,

    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_BoardColumns_CreatedAt]
            DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_BoardColumns_UpdatedAt]
            DEFAULT (SYSUTCDATETIME()),
    
    CONSTRAINT [PK_BoardColumns]
        PRIMARY KEY ([BoardColumnId]),

    CONSTRAINT [FK_BoardColumns_Boards]
        FOREIGN KEY ([BoardId])
            REFERENCES [dbo].[Boards]([BoardId])
            ON DELETE CASCADE,

    CONSTRAINT [UQ_BoardColumn_Position]
        UNIQUE ([BoardId], [Position]),

    CONSTRAINT [CK_BoardColumn_Position]
        CHECK ([Position] >= 1)
);

GO