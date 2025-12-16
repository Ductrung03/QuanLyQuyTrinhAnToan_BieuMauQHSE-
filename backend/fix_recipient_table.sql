-- Kiểm tra và sửa bảng OpsSubmissionRecipient
USE SSMS_KhaiThacTau;

-- Kiểm tra cấu trúc hiện tại
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'OpsSubmissionRecipient'
ORDER BY ORDINAL_POSITION;

-- Nếu không có cột RecipientId, thêm vào
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OpsSubmissionRecipient' AND COLUMN_NAME = 'RecipientId')
BEGIN
    -- Thêm cột RecipientId làm primary key
    ALTER TABLE OpsSubmissionRecipient ADD RecipientId INT IDENTITY(1,1) PRIMARY KEY;
END

-- Nếu có cột Id, xóa nó
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OpsSubmissionRecipient' AND COLUMN_NAME = 'Id')
BEGIN
    -- Drop primary key constraint trước
    DECLARE @ConstraintName NVARCHAR(200)
    SELECT @ConstraintName = name FROM sys.key_constraints WHERE type = 'PK' AND parent_object_id = OBJECT_ID('OpsSubmissionRecipient')
    IF @ConstraintName IS NOT NULL
        EXEC('ALTER TABLE OpsSubmissionRecipient DROP CONSTRAINT ' + @ConstraintName)
    
    -- Xóa cột Id
    ALTER TABLE OpsSubmissionRecipient DROP COLUMN Id;
END

-- Kiểm tra lại cấu trúc sau khi sửa
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'OpsSubmissionRecipient'
ORDER BY ORDINAL_POSITION;
