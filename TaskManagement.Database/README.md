# SQL Server schema deployment

`TaskManagement.Database.sqlproj` is the schema source for `dbo.Users`,
`dbo.UserSessions`, and `dbo.RefreshTokens`. EF Core maps those tables at runtime;
the API does not create the database or apply EF migrations at startup.

Run these commands from the solution root. Build the database artifact:

```powershell
dotnet build TaskManagement.Database/TaskManagement.Database.sqlproj
```

The output is `TaskManagement.Database/bin/Debug/TaskManagement.Database.dacpac`.
Use the SqlPackage CLI or the SQL Database Projects extension to deploy it.
Keep credentials in your local environment or secret store, outside Git.

Before publishing to an existing database, generate a deployment script and report
against the intended target using a locally supplied connection string:

```powershell
sqlpackage /Action:DeployReport /SourceFile:TaskManagement.Database/bin/Debug/TaskManagement.Database.dacpac /TargetConnectionString:$env:TASKMANAGEMENT_DB_CONNECTION /OutputPath:TaskManagement.Database/bin/Debug/deployment-report.xml
sqlpackage /Action:Script /SourceFile:TaskManagement.Database/bin/Debug/TaskManagement.Database.dacpac /TargetConnectionString:$env:TASKMANAGEMENT_DB_CONNECTION /OutputPath:TaskManagement.Database/bin/Debug/deployment.sql /p:BlockOnPossibleDataLoss=True /p:DropObjectsNotInSource=False
```

Review both artifacts before applying the update. This update allows null
`UserSessions.LastActivityAt` and `RevokedAt`, and adds the foreign key from
`RefreshTokens.SessionId` to `UserSessions.SessionId`. Existing orphaned token
rows will prevent that foreign key from being installed; investigate and repair
their ownership before publishing. Existing session revocation values are
preserved; changing nullability does not reactivate old sessions.

After reviewing the target and deployment script, publish explicitly:

```powershell
sqlpackage /Action:Publish /SourceFile:TaskManagement.Database/bin/Debug/TaskManagement.Database.dacpac /TargetConnectionString:$env:TASKMANAGEMENT_DB_CONNECTION /p:BlockOnPossibleDataLoss=True /p:DropObjectsNotInSource=False
```

Verify the deployed column nullability, unique constraints, foreign keys and
expiration checks in SQL Server, then run the authentication HTTP request
collections against the configured API. A successful DACPAC build alone
does not verify the deployed database or browser behavior.
