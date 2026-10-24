CREATE TABLE [dbo].[RefreshTokens](
    [RefreshTokenId] BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT [PK_RefreshTokens]
            PRIMARY KEY,

    [SessionId] UNIQUEIDENTIFIER NOT NULL,

    [TokenHash] VARCHAR(64) NOT NULL,

    [CreatedAt] DATETIME2 NOT NULL
        CONSTRAINT [DF_RefreshTokens_CreatedAt]
            DEFAULT (SYSUTCDATETIME()),
    
    [ExpiresAt] DATETIME2 NOT NULL,

    [RevokedAt] DATETIME2 NULL,

    [ReplacedByTokenId] BIGINT NULL,

    CONSTRAINT [FK_RefreshTokens_Replacement]
        FOREIGN KEY ([ReplacedByTokenId])
        REFERENCES [dbo].[RefreshTokens]([RefreshTokenId]),

    CONSTRAINT [UQ_RefreshTokens_TokenHash]
        UNIQUE ([TokenHash]),

    CONSTRAINT [CK_RefreshTokens_Expiration]
        CHECK ([ExpiresAt] > [CreatedAt])
);

GO

CREATE INDEX [IX_RefreshTokens_SessionId]
    ON [dbo].[RefreshTokens]([SessionId]);

GO