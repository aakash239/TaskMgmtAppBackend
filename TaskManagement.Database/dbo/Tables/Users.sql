CREATE TABLE [dbo].[Users](
    [Name] NVARCHAR(256) NOT NULL,
    [UserId] INT IDENTITY(1,1) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [NormalizedEmail] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(512) NOT NULL,
    
    [IsActive] BIT NOT NULL
        CONSTRAINT [DF_Users_IsActive]
        DEFAULT(1),
    
    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_Users_CreatedAt]
        DEFAULT(SYSUTCDATETIME()),
    
    CONSTRAINT [PK_Users]
        PRIMARY KEY ([UserId]),
    
    CONSTRAINT [UniqueNormalizedEmail]
        UNIQUE ([NormalizedEmail])
);