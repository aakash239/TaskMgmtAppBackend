CREATE TABLE [dbo].[ProjectMembers](
[ProjectId] INT NOT NULL,
[UserId] INT NOT NULL,

[Role] TINYINT NOT NULL
    CONSTRAINT [ProjectMembers_Role] DEFAULT(0),
[JoinedAt] DATETIME2 NOT NULL
    CONSTRAINT [ProjectMembers_JoinedAt] DEFAULT (SYSUTCDATETIME()),

CONSTRAINT [PK_ProjectMembers] PRIMARY KEY ([ProjectId], [UserId]),

CONSTRAINT [FK_ProjectMembers_Projects] FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]([ProjectId]) ON DELETE CASCADE,

CONSTRAINT [FK_ProjectMembers_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
CONSTRAINT [CK_ProjectMembers_Role] CHECK ([Role] IN (0,1))
);