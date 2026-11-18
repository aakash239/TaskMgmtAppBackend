CREATE TABLE [dbo].[Projects](
[ProjectId] INT IDENTITY(1,1) NOT NULL, 
[Name] NVARCHAR(200) NOT NULL,
[Description] NVARCHAR(2000) NULL,
[Status] TINYINT NOT NULL     -- 0 : Active, 1: In Progress, 2: Completed 
         CONSTRAINT [Projects_Status] DEFAULT(0), 
[DueDate] DATE NULL,
[CreatedByUserId] INT NOT NULL,
[CreatedAt] DATETIME2 NOT NULL
   CONSTRAINT [Projects_CreatedAt] DEFAULT(SYSUTCDATETIME()),
[UpdatedAt] DATETIME2 NULL,

[IsDeleted] BIT NOT NULL
   CONSTRAINT  [Projects_IsDeleted] DEFAULT(0),
[DeletedAt] DATETIME2 NULL,
[DeletedByUserId] INT NULL,

CONSTRAINT [PK_Projects] PRIMARY KEY ([ProjectId]),
CONSTRAINT [FK_Projects_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users]([UserId]),
CONSTRAINT [FK_Projects_DeletedBy] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users]([UserId]),
CONSTRAINT [CK_Projects_Status] CHECK ([Status] IN (0,1,2))

);





