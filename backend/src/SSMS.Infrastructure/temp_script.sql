CREATE TABLE [Unit] (
    [UnitId] int NOT NULL IDENTITY,
    [UnitCode] nvarchar(50) NULL,
    [UnitName] nvarchar(200) NULL,
    [UnitType] nvarchar(50) NULL,
    [Description] nvarchar(500) NULL,
    [ParentUnitId] int NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2(0) NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Unit] PRIMARY KEY ([UnitId]),
    CONSTRAINT [FK_Unit_Unit_ParentUnitId] FOREIGN KEY ([ParentUnitId]) REFERENCES [Unit] ([UnitId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [AppUser] (
    [UserId] int NOT NULL IDENTITY,
    [UserName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NULL,
    [FullName] nvarchar(200) NULL,
    [PasswordHash] nvarchar(500) NULL,
    [Phone] nvarchar(50) NULL,
    [Position] nvarchar(100) NULL,
    [UnitId] int NULL,
    [Role] nvarchar(50) NULL DEFAULT N'User',
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [LastLoginAt] datetime2 NULL,
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2(0) NULL,
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_AppUser] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_AppUser_Unit_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Unit] ([UnitId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [OpsAuditLog] (
    [LogId] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [UserName] nvarchar(200) NULL,
    [Action] nvarchar(100) NOT NULL,
    [TargetType] nvarchar(50) NULL,
    [TargetId] int NULL,
    [TargetName] nvarchar(500) NULL,
    [Detail] nvarchar(1000) NULL,
    [OldData] nvarchar(max) NULL,
    [NewData] nvarchar(max) NULL,
    [IpAddress] nvarchar(50) NULL,
    [UserAgent] nvarchar(500) NULL,
    [ActionTime] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsAuditLog] PRIMARY KEY ([LogId]),
    CONSTRAINT [FK_OpsAuditLog_AppUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [OpsProcedure] (
    [ProcedureId] int NOT NULL IDENTITY,
    [Code] nvarchar(20) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [OwnerUserId] int NULL,
    [Version] nvarchar(20) NULL,
    [State] nvarchar(30) NOT NULL DEFAULT N'Draft',
    [AuthorUserId] int NULL,
    [ApproverUserId] int NULL,
    [CreatedDate] datetime2 NULL,
    [ReleasedDate] datetime2 NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2(0) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsProcedure] PRIMARY KEY ([ProcedureId]),
    CONSTRAINT [FK_OpsProcedure_AppUser_ApproverUserId] FOREIGN KEY ([ApproverUserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OpsProcedure_AppUser_AuthorUserId] FOREIGN KEY ([AuthorUserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OpsProcedure_AppUser_OwnerUserId] FOREIGN KEY ([OwnerUserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [OpsProcedureDocument] (
    [ProcedureDocId] int NOT NULL IDENTITY,
    [ProcedureId] int NOT NULL,
    [DocVersion] nvarchar(20) NULL,
    [FileName] nvarchar(255) NOT NULL,
    [FilePath] nvarchar(500) NULL,
    [FileSize] bigint NULL,
    [ContentType] nvarchar(100) NULL,
    [UploadedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsProcedureDocument] PRIMARY KEY ([ProcedureDocId]),
    CONSTRAINT [FK_OpsProcedureDocument_OpsProcedure_ProcedureId] FOREIGN KEY ([ProcedureId]) REFERENCES [OpsProcedure] ([ProcedureId]) ON DELETE CASCADE
);
GO


CREATE TABLE [OpsTemplate] (
    [TemplateId] int NOT NULL IDENTITY,
    [TemplateKey] nvarchar(32) NULL,
    [ProcedureId] int NOT NULL,
    [TemplateNo] nvarchar(50) NULL,
    [Name] nvarchar(255) NOT NULL,
    [TemplateType] nvarchar(30) NOT NULL DEFAULT N'Form',
    [State] nvarchar(30) NOT NULL DEFAULT N'Draft',
    [FileName] nvarchar(255) NULL,
    [FilePath] nvarchar(500) NULL,
    [FileSize] bigint NULL,
    [ContentType] nvarchar(100) NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsTemplate] PRIMARY KEY ([TemplateId]),
    CONSTRAINT [FK_OpsTemplate_OpsProcedure_ProcedureId] FOREIGN KEY ([ProcedureId]) REFERENCES [OpsProcedure] ([ProcedureId]) ON DELETE CASCADE
);
GO


CREATE TABLE [OpsSubmission] (
    [SubmissionId] int NOT NULL IDENTITY,
    [ProcedureId] int NOT NULL,
    [TemplateId] int NULL,
    [SubmittedByUserId] int NOT NULL,
    [SubmittedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [Status] nvarchar(30) NOT NULL DEFAULT N'Submitted',
    [Title] nvarchar(255) NOT NULL,
    [Content] nvarchar(max) NULL,
    [RecalledAt] datetime2 NULL,
    [RecallReason] nvarchar(500) NULL,
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsSubmission] PRIMARY KEY ([SubmissionId]),
    CONSTRAINT [FK_OpsSubmission_AppUser_SubmittedByUserId] FOREIGN KEY ([SubmittedByUserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OpsSubmission_OpsProcedure_ProcedureId] FOREIGN KEY ([ProcedureId]) REFERENCES [OpsProcedure] ([ProcedureId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OpsSubmission_OpsTemplate_TemplateId] FOREIGN KEY ([TemplateId]) REFERENCES [OpsTemplate] ([TemplateId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [OpsApproval] (
    [ApprovalId] int NOT NULL IDENTITY,
    [SubmissionId] int NOT NULL,
    [ApproverUserId] int NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [Note] nvarchar(500) NULL,
    [ActionDate] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsApproval] PRIMARY KEY ([ApprovalId]),
    CONSTRAINT [FK_OpsApproval_AppUser_ApproverUserId] FOREIGN KEY ([ApproverUserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OpsApproval_OpsSubmission_SubmissionId] FOREIGN KEY ([SubmissionId]) REFERENCES [OpsSubmission] ([SubmissionId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [OpsSubmissionFile] (
    [SubmissionFileId] int NOT NULL IDENTITY,
    [SubmissionId] int NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [FilePath] nvarchar(500) NOT NULL,
    [FileSize] bigint NOT NULL,
    [ContentType] nvarchar(100) NULL,
    [UploadedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsSubmissionFile] PRIMARY KEY ([SubmissionFileId]),
    CONSTRAINT [FK_OpsSubmissionFile_OpsSubmission_SubmissionId] FOREIGN KEY ([SubmissionId]) REFERENCES [OpsSubmission] ([SubmissionId]) ON DELETE CASCADE
);
GO


CREATE TABLE [OpsSubmissionRecipient] (
    [RecipientId] int NOT NULL IDENTITY,
    [SubmissionId] int NOT NULL,
    [RecipientUserId] int NOT NULL,
    [RecipientType] nvarchar(10) NOT NULL DEFAULT N'CC',
    [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ReadAt] datetime2(0) NULL,
    [CreatedAt] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OpsSubmissionRecipient] PRIMARY KEY ([RecipientId]),
    CONSTRAINT [FK_OpsSubmissionRecipient_AppUser_RecipientUserId] FOREIGN KEY ([RecipientUserId]) REFERENCES [AppUser] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OpsSubmissionRecipient_OpsSubmission_SubmissionId] FOREIGN KEY ([SubmissionId]) REFERENCES [OpsSubmission] ([SubmissionId]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_AppUser_UnitId] ON [AppUser] ([UnitId]);
GO


CREATE INDEX [IX_OpsApproval_ApproverUserId] ON [OpsApproval] ([ApproverUserId]);
GO


CREATE INDEX [IX_OpsApproval_SubmissionId] ON [OpsApproval] ([SubmissionId]);
GO


CREATE INDEX [IX_OpsAuditLog_Action] ON [OpsAuditLog] ([Action]);
GO


CREATE INDEX [IX_OpsAuditLog_ActionTime] ON [OpsAuditLog] ([ActionTime]);
GO


CREATE INDEX [IX_OpsAuditLog_TargetType] ON [OpsAuditLog] ([TargetType]);
GO


CREATE INDEX [IX_OpsAuditLog_UserId] ON [OpsAuditLog] ([UserId]);
GO


CREATE INDEX [IX_OpsProcedure_ApproverUserId] ON [OpsProcedure] ([ApproverUserId]);
GO


CREATE INDEX [IX_OpsProcedure_AuthorUserId] ON [OpsProcedure] ([AuthorUserId]);
GO


CREATE UNIQUE INDEX [IX_OpsProcedure_Code] ON [OpsProcedure] ([Code]);
GO


CREATE INDEX [IX_OpsProcedure_OwnerUserId] ON [OpsProcedure] ([OwnerUserId]);
GO


CREATE INDEX [IX_OpsProcedureDocument_ProcedureId] ON [OpsProcedureDocument] ([ProcedureId]);
GO


CREATE INDEX [IX_OpsSubmission_ProcedureId] ON [OpsSubmission] ([ProcedureId]);
GO


CREATE INDEX [IX_OpsSubmission_Status] ON [OpsSubmission] ([Status]);
GO


CREATE INDEX [IX_OpsSubmission_SubmittedByUserId] ON [OpsSubmission] ([SubmittedByUserId]);
GO


CREATE INDEX [IX_OpsSubmission_TemplateId] ON [OpsSubmission] ([TemplateId]);
GO


CREATE INDEX [IX_OpsSubmissionFile_SubmissionId] ON [OpsSubmissionFile] ([SubmissionId]);
GO


CREATE INDEX [IX_OpsSubmissionRecipient_RecipientUserId] ON [OpsSubmissionRecipient] ([RecipientUserId]);
GO


CREATE INDEX [IX_OpsSubmissionRecipient_SubmissionId] ON [OpsSubmissionRecipient] ([SubmissionId]);
GO


CREATE INDEX [IX_OpsTemplate_ProcedureId] ON [OpsTemplate] ([ProcedureId]);
GO


CREATE INDEX [IX_Unit_ParentUnitId] ON [Unit] ([ParentUnitId]);
GO


