-- Add missing BaseEntity columns to OpsProcedure table if they don't exist

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE OpsProcedure ADD IsDeleted bit NOT NULL DEFAULT 0
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE OpsProcedure ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME()
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE OpsProcedure ADD UpdatedAt datetime2 NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE OpsProcedure ADD CreatedBy nvarchar(max) NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE OpsProcedure ADD UpdatedBy nvarchar(max) NULL
END

-- Add missing columns to OpsProcedureDocument
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE OpsProcedureDocument ADD IsDeleted bit NOT NULL DEFAULT 0
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE OpsProcedureDocument ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME()
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE OpsProcedureDocument ADD UpdatedAt datetime2 NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE OpsProcedureDocument ADD CreatedBy nvarchar(max) NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE OpsProcedureDocument ADD UpdatedBy nvarchar(max) NULL
END

-- Add missing columns to OpsTemplate
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE OpsTemplate ADD IsDeleted bit NOT NULL DEFAULT 0
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE OpsTemplate ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME()
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE OpsTemplate ADD UpdatedAt datetime2 NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE OpsTemplate ADD CreatedBy nvarchar(max) NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE OpsTemplate ADD UpdatedBy nvarchar(max) NULL
END

-- Add to OpsSubmission if needed
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE OpsSubmission ADD IsDeleted bit NOT NULL DEFAULT 0
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE OpsSubmission ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME()
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE OpsSubmission ADD UpdatedAt datetime2 NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE OpsSubmission ADD CreatedBy nvarchar(max) NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE OpsSubmission ADD UpdatedBy nvarchar(max) NULL
END

PRINT 'Database schema updated successfully!'
