-- Thêm cột ContentType và FileSize vào bảng OpsTemplate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsTemplate]') AND name = 'ContentType')
BEGIN
    ALTER TABLE [OpsTemplate] ADD [ContentType] NVARCHAR(100) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsTemplate]') AND name = 'FileSize')
BEGIN
    ALTER TABLE [OpsTemplate] ADD [FileSize] BIGINT NULL;
END
