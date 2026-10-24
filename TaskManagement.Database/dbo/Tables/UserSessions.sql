CREATE TABLE [dbo].[UserSessions](
    [SessionId] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT [PK_UserSessions]
            PRIMARY KEY,

    [UserId] INT NOT NULL,

    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_UserSessions_CreatedAt]
            DEFAULT (SYSUTCDATETIME()),

    [ExpiresAt] DATETIME2 NOT NULL,

    [LastActivityAt] DATETIME2 NOT NULL,

    [RevokedAt] DATETIME2 NOT NULL,

    CONSTRAINT [FK_UserSessions_Users]
        FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users]([UserId]),

    CONSTRAINT [CK_UserSessions_Expiration]
        CHECK ([ExpiresAt] > ([CreatedAt]))
);

GO

CREATE INDEX [IX_UserSessions_UserId]
    ON [dbo].[UserSessions]([UserId]);

GO

