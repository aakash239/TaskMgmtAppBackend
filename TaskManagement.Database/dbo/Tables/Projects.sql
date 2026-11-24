CREATE TABLE [dbo].[Projects] (
   [ProjectId]       INT             IDENTITY (1, 1) NOT NULL,
   [Name]            NVARCHAR (200)  NOT NULL,
   [TemplateId]      INT             NOT NULL,
   [Description]     NVARCHAR (2000) NULL,
   [Status]          TINYINT         CONSTRAINT [Projects_Status] DEFAULT (0) NOT NULL, -- 0 : Active, 1: In Progress, 2: Completed 
   [DueDate]         DATE            NULL,
   [CreatedByUserId] INT             NOT NULL,
   [CreatedAt]       DATETIME2       CONSTRAINT [Projects_CreatedAt] DEFAULT (SYSUTCDATETIME()) NOT NULL,
   [UpdatedAt]       DATETIME2       NULL,
   [IsDeleted]       BIT             CONSTRAINT [Projects_IsDeleted] DEFAULT (0) NOT NULL,
   [DeletedAt]       DATETIME2       NULL,
   [DeletedByUserId] INT             NULL,
   CONSTRAINT [PK_Projects] PRIMARY KEY ([ProjectId]),
   CONSTRAINT [FK_Projects_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
   CONSTRAINT [FK_Projects_DeletedBy] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
   CONSTRAINT [FK_Projects_KanbanTemplates] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Users] ([UserId]),
   CONSTRAINT [CK_Projects_Status] CHECK ([Status] IN (0, 1, 2))
);