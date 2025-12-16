-- Migration script for OpsAuditLog table
-- Run this script to create the audit log table in SSMS_KhaiThacTau database

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OpsAuditLog' AND xtype='U')
BEGIN
    CREATE TABLE OpsAuditLog (
        LogId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NULL,
        UserName NVARCHAR(200) NULL,
        Action NVARCHAR(100) NOT NULL,
        TargetType NVARCHAR(50) NULL,
        TargetId INT NULL,
        TargetName NVARCHAR(500) NULL,
        Detail NVARCHAR(1000) NULL,
        OldData NVARCHAR(MAX) NULL,
        NewData NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        ActionTime DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
        CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NULL,
        CreatedBy NVARCHAR(100) NULL,
        UpdatedBy NVARCHAR(100) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        
        CONSTRAINT FK_OpsAuditLog_User FOREIGN KEY (UserId) 
            REFERENCES AppUser(UserId) ON DELETE NO ACTION
    );

    -- Create indexes for better query performance
    CREATE INDEX IX_OpsAuditLog_UserId ON OpsAuditLog(UserId);
    CREATE INDEX IX_OpsAuditLog_Action ON OpsAuditLog(Action);
    CREATE INDEX IX_OpsAuditLog_TargetType ON OpsAuditLog(TargetType);
    CREATE INDEX IX_OpsAuditLog_ActionTime ON OpsAuditLog(ActionTime DESC);

    PRINT 'Created OpsAuditLog table successfully';
END
ELSE
BEGIN
    PRINT 'OpsAuditLog table already exists';
END
GO

-- Insert sample audit log entries for testing
INSERT INTO OpsAuditLog (UserId, UserName, Action, TargetType, TargetId, TargetName, Detail, ActionTime)
VALUES 
(1, 'Admin System', 'Login', 'User', 1, 'Admin', 'Đăng nhập hệ thống', DATEADD(HOUR, -2, SYSUTCDATETIME())),
(1, 'Admin System', 'Create', 'Procedure', 1, 'OPS-01', 'Tạo quy trình mới', DATEADD(HOUR, -1, SYSUTCDATETIME())),
(2, 'Nguyen Van A', 'Submit', 'Submission', 1, 'SUB-00001', 'Nộp biểu mẫu FM-OPS-01-01', DATEADD(MINUTE, -30, SYSUTCDATETIME()));

PRINT 'Inserted sample audit log entries';
GO
