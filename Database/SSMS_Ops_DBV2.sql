/*
    SSMS – Modul Quản lý Khai thác tàu
    T-SQL script thiết kế CSDL MS SQL Server (phiên bản demo)

    Gồm các nhóm bảng:
      - Danh mục dùng chung: AppUser, Unit
      - Quy trình & tài liệu: OpsProcedure, OpsProcedureDocument, OpsProcedureChange, OpsTemplate
      - Nộp biểu mẫu & phê duyệt: OpsSubmission, OpsSubmissionRecipient, OpsSubmissionFile, OpsApproval
      - Nhật ký hệ thống: OpsAuditLog
*/

------------------------------------------------------------
-- 1. (Tùy chọn) Tạo CSDL riêng cho modul
------------------------------------------------------------
IF DB_ID(N'SSMS_KhaiThacTau') IS NULL
BEGIN
    CREATE DATABASE SSMS_KhaiThacTau;
END;
GO

USE SSMS_KhaiThacTau;
GO

------------------------------------------------------------
-- 2. Danh mục dùng chung
------------------------------------------------------------

-- Người dùng hệ thống
IF OBJECT_ID(N'dbo.AppUser', N'U') IS NOT NULL DROP TABLE dbo.AppUser;
GO
CREATE TABLE dbo.AppUser (
    UserId      INT IDENTITY(1,1) CONSTRAINT PK_AppUser PRIMARY KEY,
    UserName    NVARCHAR(100)  NOT NULL,   -- Ví dụ: 'Nguyễn Văn A'
    LoginName   NVARCHAR(100)  NULL,       -- nếu cần map sang tài khoản AD
    Email       NVARCHAR(255)  NULL,
    Phone       NVARCHAR(50)   NULL,
    IsActive    BIT            NOT NULL CONSTRAINT DF_AppUser_IsActive DEFAULT(1),
    CreatedAt   DATETIME2(0)   NOT NULL CONSTRAINT DF_AppUser_CreatedAt DEFAULT (SYSUTCDATETIME())
);
GO

-- Đơn vị / Tàu / Phòng ban
IF OBJECT_ID(N'dbo.Unit', N'U') IS NOT NULL DROP TABLE dbo.Unit;
GO
CREATE TABLE dbo.Unit (
    UnitId      INT IDENTITY(1,1) CONSTRAINT PK_Unit PRIMARY KEY,
    UnitName    NVARCHAR(200) NOT NULL,   -- Ví dụ: 'Tàu Petrolimex 15', 'Phòng Kỹ thuật'
    UnitCode    NVARCHAR(50)  NULL,       -- nếu muốn đặt mã riêng
    UnitType    NVARCHAR(50)  NULL,       -- 'Ship', 'Dept', 'Board', ...
    IsActive    BIT           NOT NULL CONSTRAINT DF_Unit_IsActive DEFAULT(1),
    SortOrder   INT           NULL
);
GO

------------------------------------------------------------
-- 3. Quy trình & tài liệu
------------------------------------------------------------

-- Bảng Quy trình (OPS-01, OPS-02, ...)
IF OBJECT_ID(N'dbo.OpsProcedure', N'U') IS NOT NULL DROP TABLE dbo.OpsProcedure;
GO
CREATE TABLE dbo.OpsProcedure (
    ProcedureId     INT IDENTITY(1,1) CONSTRAINT PK_OpsProcedure PRIMARY KEY,
    Code            NVARCHAR(20)  NOT NULL,      -- ví dụ: 'OPS-01'
    Name            NVARCHAR(255) NOT NULL,      -- tên quy trình
    OwnerUserId     INT           NULL,          -- Chủ trì
    Version         NVARCHAR(20)  NULL,          -- '1.0', '0.8'...
    State           NVARCHAR(30)  NOT NULL,      -- Draft / Submitted / Approved / ...
    AuthorUserId    INT           NULL,          -- Người lập
    ApproverUserId  INT           NULL,          -- Người duyệt
    CreatedDate     DATE          NULL,
    ReleasedDate    DATE          NULL,
    Description     NVARCHAR(MAX) NULL,          -- mô tả song ngữ, mục đích, phạm vi...
    CONSTRAINT UQ_OpsProcedure_Code UNIQUE (Code),
    CONSTRAINT FK_OpsProcedure_Owner      FOREIGN KEY (OwnerUserId)    REFERENCES dbo.AppUser(UserId),
    CONSTRAINT FK_OpsProcedure_Author     FOREIGN KEY (AuthorUserId)   REFERENCES dbo.AppUser(UserId),
    CONSTRAINT FK_OpsProcedure_Approver   FOREIGN KEY (ApproverUserId) REFERENCES dbo.AppUser(UserId)
);
GO

-- Tài liệu đính kèm cho từng quy trình (DOCX/PDF...)
IF OBJECT_ID(N'dbo.OpsProcedureDocument', N'U') IS NOT NULL DROP TABLE dbo.OpsProcedureDocument;
GO
CREATE TABLE dbo.OpsProcedureDocument (
    ProcedureDocId  INT IDENTITY(1,1) CONSTRAINT PK_OpsProcedureDocument PRIMARY KEY,
    ProcedureId     INT           NOT NULL,
    DocVersion      NVARCHAR(20)  NULL,          -- '1.0'
    FileName        NVARCHAR(255) NOT NULL,      -- tên file gốc
    FilePath        NVARCHAR(500) NULL,          -- đường dẫn lưu trữ thực tế nếu có
    UploadedAt      DATETIME2(0)  NOT NULL CONSTRAINT DF_OpsProcedureDocument_UploadedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_OpsProcedureDocument_Procedure FOREIGN KEY (ProcedureId)
        REFERENCES dbo.OpsProcedure(ProcedureId)
);
GO

-- Nhật ký thay đổi version của quy trình
IF OBJECT_ID(N'dbo.OpsProcedureChange', N'U') IS NOT NULL DROP TABLE dbo.OpsProcedureChange;
GO
CREATE TABLE dbo.OpsProcedureChange (
    ProcedureChangeId   INT IDENTITY(1,1) CONSTRAINT PK_OpsProcedureChange PRIMARY KEY,
    ProcedureId         INT           NOT NULL,
    ChangeTime          DATETIME2(0)  NOT NULL,
    UserId              INT           NULL,
    Action              NVARCHAR(50)  NOT NULL,      -- Create / Edit / Approve / ...
    Detail              NVARCHAR(1000) NULL,
    CONSTRAINT FK_OpsProcedureChange_Procedure FOREIGN KEY (ProcedureId)
        REFERENCES dbo.OpsProcedure(ProcedureId),
    CONSTRAINT FK_OpsProcedureChange_User      FOREIGN KEY (UserId)
        REFERENCES dbo.AppUser(UserId)
);
GO

-- Biểu mẫu / Checklist gắn với từng quy trình
IF OBJECT_ID(N'dbo.OpsTemplate', N'U') IS NOT NULL DROP TABLE dbo.OpsTemplate;
GO
CREATE TABLE dbo.OpsTemplate (
    TemplateId      INT IDENTITY(1,1) CONSTRAINT PK_OpsTemplate PRIMARY KEY,
    TemplateKey     NVARCHAR(32)  NULL,          -- id trong JSON: 'T1', 'T6ihr'...
    ProcedureId     INT           NOT NULL,
    TemplateNo      NVARCHAR(50)  NULL,          -- ví dụ: 'FM-OPS-01', 'SOF 05-04-01'
    Name            NVARCHAR(255) NOT NULL,      -- tên biểu mẫu
    TemplateType    NVARCHAR(30)  NOT NULL,      -- 'Form' / 'Checklist'
    State           NVARCHAR(30)  NOT NULL,      -- Draft / Submitted / Approved / ...
    FileName        NVARCHAR(255) NULL,          -- file mẫu DOCX/XLSX/PDF
    FilePath        NVARCHAR(500) NULL,
    IsActive        BIT           NOT NULL CONSTRAINT DF_OpsTemplate_IsActive DEFAULT(1),
    CONSTRAINT FK_OpsTemplate_Procedure FOREIGN KEY (ProcedureId)
        REFERENCES dbo.OpsProcedure(ProcedureId)
);
GO

CREATE INDEX IX_OpsTemplate_Procedure ON dbo.OpsTemplate(ProcedureId);
GO

------------------------------------------------------------
-- 4. Nộp biểu mẫu & phê duyệt
------------------------------------------------------------

-- Phiếu nộp biểu mẫu (1 record tương ứng 1 lần nộp 1 mẫu)
IF OBJECT_ID(N'dbo.OpsSubmission', N'U') IS NOT NULL DROP TABLE dbo.OpsSubmission;
GO
CREATE TABLE dbo.OpsSubmission (
    SubmissionId        INT IDENTITY(1,1) CONSTRAINT PK_OpsSubmission PRIMARY KEY,
    SubmissionCode      NVARCHAR(32)  NOT NULL,      -- ví dụ: 'SUB-4YGHSE'
    ProcedureId         INT           NOT NULL,
    TemplateId          INT           NULL,
    UnitId              INT           NOT NULL,      -- đơn vị gửi: Tàu PLX15, Phòng Khai thác...
    SenderUserId        INT           NOT NULL,      -- người gửi
    SentAt              DATETIME2(0)  NULL,          -- thời điểm nộp (Submitted)
    DueDate             DATE          NULL,          -- hạn xử lý
    State               NVARCHAR(30)  NOT NULL,      -- Draft / Submitted / Approved / Rejected...
    Note                NVARCHAR(MAX) NULL,          -- ghi chú gửi người duyệt
    CreatedAt           DATETIME2(0)  NOT NULL CONSTRAINT DF_OpsSubmission_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT UQ_OpsSubmission_Code UNIQUE (SubmissionCode),
    CONSTRAINT FK_OpsSubmission_Procedure FOREIGN KEY (ProcedureId)
        REFERENCES dbo.OpsProcedure(ProcedureId),
    CONSTRAINT FK_OpsSubmission_Template  FOREIGN KEY (TemplateId)
        REFERENCES dbo.OpsTemplate(TemplateId),
    CONSTRAINT FK_OpsSubmission_Unit      FOREIGN KEY (UnitId)
        REFERENCES dbo.Unit(UnitId),
    CONSTRAINT FK_OpsSubmission_Sender    FOREIGN KEY (SenderUserId)
        REFERENCES dbo.AppUser(UserId)
);
GO

-- Danh sách người nhận / phòng ban nhận biểu mẫu (nhiều-đến-nhiều)
IF OBJECT_ID(N'dbo.OpsSubmissionRecipient', N'U') IS NOT NULL DROP TABLE dbo.OpsSubmissionRecipient;
GO
CREATE TABLE dbo.OpsSubmissionRecipient (
    SubmissionId    INT NOT NULL,
    UnitId          INT NOT NULL,          -- Phòng Kỹ thuật, Phòng An toàn/QHSE, Ban Giám đốc...
    RecipientRole   NVARCHAR(30) NULL,     -- To / Cc (dự phòng)
    CONSTRAINT PK_OpsSubmissionRecipient PRIMARY KEY (SubmissionId, UnitId),
    CONSTRAINT FK_OpsSubmissionRecipient_Sub FOREIGN KEY (SubmissionId)
        REFERENCES dbo.OpsSubmission(SubmissionId),
    CONSTRAINT FK_OpsSubmissionRecipient_Unit FOREIGN KEY (UnitId)
        REFERENCES dbo.Unit(UnitId)
);
GO

-- Các file biểu mẫu đã điền kèm theo 1 lần nộp
IF OBJECT_ID(N'dbo.OpsSubmissionFile', N'U') IS NOT NULL DROP TABLE dbo.OpsSubmissionFile;
GO
CREATE TABLE dbo.OpsSubmissionFile (
    SubmissionFileId INT IDENTITY(1,1) CONSTRAINT PK_OpsSubmissionFile PRIMARY KEY,
    SubmissionId     INT           NOT NULL,
    FileName         NVARCHAR(255) NOT NULL,
    FilePath         NVARCHAR(500) NULL,
    UploadedAt       DATETIME2(0)  NOT NULL CONSTRAINT DF_OpsSubmissionFile_UploadedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_OpsSubmissionFile_Sub FOREIGN KEY (SubmissionId)
        REFERENCES dbo.OpsSubmission(SubmissionId)
);
GO

-- Hàng phê duyệt tương ứng với các submission
IF OBJECT_ID(N'dbo.OpsApproval', N'U') IS NOT NULL DROP TABLE dbo.OpsApproval;
GO
CREATE TABLE dbo.OpsApproval (
    ApprovalId      INT IDENTITY(1,1) CONSTRAINT PK_OpsApproval PRIMARY KEY,
    ApprovalCode    NVARCHAR(32)  NOT NULL,      -- ví dụ: 'APR-SUB-4YGHSE'
    SubmissionId    INT           NULL,
    Kind            NVARCHAR(100) NOT NULL,      -- 'Biểu mẫu đã nộp'...
    Name            NVARCHAR(255) NOT NULL,      -- tên form: 'Kế hoạch chuyến (Form)'
    ProcedureId     INT           NULL,
    SenderUnitId    INT           NULL,
    RequestedAt     DATETIME2(0)  NULL,          -- ngày tạo hàng chờ
    State           NVARCHAR(30)  NOT NULL,      -- Submitted / Approved / Rejected
    ApproverUserId  INT           NULL,
    DecisionAt      DATETIME2(0)  NULL,          -- thời điểm phê duyệt / từ chối
    Comment         NVARCHAR(MAX) NULL,          -- nhận xét của người duyệt
    CONSTRAINT UQ_OpsApproval_Code UNIQUE (ApprovalCode),
    CONSTRAINT FK_OpsApproval_Submission  FOREIGN KEY (SubmissionId)
        REFERENCES dbo.OpsSubmission(SubmissionId),
    CONSTRAINT FK_OpsApproval_Procedure   FOREIGN KEY (ProcedureId)
        REFERENCES dbo.OpsProcedure(ProcedureId),
    CONSTRAINT FK_OpsApproval_SenderUnit  FOREIGN KEY (SenderUnitId)
        REFERENCES dbo.Unit(UnitId),
    CONSTRAINT FK_OpsApproval_Approver    FOREIGN KEY (ApproverUserId)
        REFERENCES dbo.AppUser(UserId)
);
GO

CREATE INDEX IX_OpsApproval_State ON dbo.OpsApproval(State);
GO

------------------------------------------------------------
-- 5. Nhật ký hệ thống
------------------------------------------------------------

IF OBJECT_ID(N'dbo.OpsAuditLog', N'U') IS NOT NULL DROP TABLE dbo.OpsAuditLog;
GO
CREATE TABLE dbo.OpsAuditLog (
    LogId       BIGINT IDENTITY(1,1) CONSTRAINT PK_OpsAuditLog PRIMARY KEY,
    LoggedAt    DATETIME2(0)  NOT NULL CONSTRAINT DF_OpsAuditLog_LoggedAt DEFAULT (SYSUTCDATETIME()),
    UserId      INT           NULL,
    Action      NVARCHAR(100) NOT NULL,   -- Submit form / Add template / Edit / Approve / ...
    Target      NVARCHAR(255) NULL,       -- tên đối tượng: 'OPS-05', 'SOF 05-04-01 ...'
    Detail      NVARCHAR(MAX) NULL,
    CONSTRAINT FK_OpsAuditLog_User FOREIGN KEY (UserId)
        REFERENCES dbo.AppUser(UserId)
);
GO

CREATE INDEX IX_OpsAuditLog_LoggedAt ON dbo.OpsAuditLog(LoggedAt DESC);
GO

------------------------------------------------------------
-- 6. Dữ liệu demo khởi tạo (tương đương JSON giao diện)
------------------------------------------------------------

-- Người dùng demo
INSERT INTO dbo.AppUser (UserName)
VALUES (N'Nguyễn Văn A'),
       (N'Trần Thị B'),
       (N'Lê Văn C'),
       (N'Phạm D'),
       (N'Vũ E');

-- Đơn vị demo
INSERT INTO dbo.Unit (UnitName, UnitType)
VALUES (N'Tàu Petrolimex 15', N'Ship'),
       (N'Tàu Petrolimex 16', N'Ship'),
       (N'Phòng Khai thác',   N'Dept'),
       (N'Phòng Kỹ thuật',    N'Dept'),
       (N'Phòng An toàn/QHSE',N'Dept'),
       (N'Phòng Tài chính',   N'Dept'),
       (N'Ban Giám đốc',      N'Board');

------------------------------------------------------------
-- Quy trình mẫu (rút gọn từ giao diện)
------------------------------------------------------------

DECLARE @A INT = (SELECT UserId FROM dbo.AppUser WHERE UserName = N'Nguyễn Văn A');
DECLARE @B INT = (SELECT UserId FROM dbo.AppUser WHERE UserName = N'Trần Thị B');
DECLARE @C INT = (SELECT UserId FROM dbo.AppUser WHERE UserName = N'Lê Văn C');
DECLARE @D INT = (SELECT UserId FROM dbo.AppUser WHERE UserName = N'Phạm D');
DECLARE @E INT = (SELECT UserId FROM dbo.AppUser WHERE UserName = N'Vũ E');

-- OPS-01 VẬN ĐƠN - BILL OF LADING
INSERT INTO dbo.OpsProcedure
    (Code, Name, OwnerUserId, Version, State, AuthorUserId, ApproverUserId, CreatedDate, ReleasedDate, Description)
VALUES
    (N'OPS-01', N'VẬN ĐƠN - BILL OF LADING', @A, N'1.0', N'Approved', @A, @E, '2025-01-05', '2025-01-10',
     N'Quy định rõ quy trình kiểm tra và ký phát Vận đơn. To clearly define the procedure for checking and signing Bills of Lading.');

-- OPS-02 HỢP ĐỒNG VẬN TẢI - CHARTER PARTY
INSERT INTO dbo.OpsProcedure
    (Code, Name, OwnerUserId, Version, State, AuthorUserId, ApproverUserId, CreatedDate, ReleasedDate, Description)
VALUES
    (N'OPS-02', N'HỢP ĐỒNG VẬN TẢI - CHARTER PARTY', @B, N'0.8', N'Draft', @B, NULL, '2025-02-01', NULL,
     N'Quy định hướng dẫn cụ thể đối với tàu biển cho thuê. To specify guidelines for vessels on charter.');

-- OPS-03 QUY TRÌNH LÀM HÀNG - CARGO OPERATION PROCEDURE
INSERT INTO dbo.OpsProcedure
    (Code, Name, OwnerUserId, Version, State, AuthorUserId, ApproverUserId, CreatedDate, ReleasedDate, Description)
VALUES
    (N'OPS-03', N'QUY TRÌNH LÀM HÀNG - CARGO OPERATION PROCEDURE', @C, N'1.0', N'Submitted', @D, @E, '2025-03-10', '2025-03-16',
     N'Đảm bảo kiểm soát các hoạt động của tàu, nâng cao an toàn trong công việc làm hàng và ngăn ngừa ô nhiễm.');

-- OPS-04 HỆ THỐNG KIỂM SOÁT HƠI HÀNG - VEC SYSTEMS
INSERT INTO dbo.OpsProcedure
    (Code, Name, OwnerUserId, Version, State, AuthorUserId, ApproverUserId, CreatedDate, ReleasedDate, Description)
VALUES
    (N'OPS-04', N'HỆ THỐNG KIỂM SOÁT HƠI HÀNG CARGO VAPOUR EMISSIONS CONTROL - VEC SYSTEMS', @A, N'1.0', N'Draft', @D, NULL, '2025-10-29', '2025-10-29',
     N'Mô tả hệ thống thu hồi hơi hàng và các quy định phải tuân thủ.');

-- OPS-05 VỆ SINH KÉT - TANK CLEANING
INSERT INTO dbo.OpsProcedure
    (Code, Name, OwnerUserId, Version, State, AuthorUserId, ApproverUserId, CreatedDate, ReleasedDate, Description)
VALUES
    (N'OPS-05', N'VỆ SINH KÉT - TANK CLEANING', @A, N'1.0', N'Draft', @D, NULL, '2025-10-29', '2025-10-29',
     N'Quy định về rửa két, tẩy khí và đưa người vào két – một trong những công việc nguy hiểm nhất trong khai thác tàu dầu.');

------------------------------------------------------------
-- Tài liệu đính kèm quy trình (ví dụ)
------------------------------------------------------------

INSERT INTO dbo.OpsProcedureDocument (ProcedureId, DocVersion, FileName, UploadedAt)
SELECT ProcedureId, N'1.0', N'OPS-01_KeHoachChuyen_v1.0.pdf',  '2025-01-10T09:00'
FROM dbo.OpsProcedure WHERE Code = N'OPS-01';

INSERT INTO dbo.OpsProcedureDocument (ProcedureId, DocVersion, FileName, UploadedAt)
SELECT ProcedureId, N'0.8', N'OPS-02_BanGiaoCa_v0.8.docx',  '2025-02-03T08:00'
FROM dbo.OpsProcedure WHERE Code = N'OPS-02';

INSERT INTO dbo.OpsProcedureDocument (ProcedureId, DocVersion, FileName, UploadedAt)
SELECT ProcedureId, N'1.0', N'OPS-03_TiepNhanNhienLieu_v1.0.pdf',  '2025-03-16T10:20'
FROM dbo.OpsProcedure WHERE Code = N'OPS-03';

------------------------------------------------------------
-- Biểu mẫu / checklist demo
------------------------------------------------------------

DECLARE @OPS01 INT = (SELECT ProcedureId FROM dbo.OpsProcedure WHERE Code = N'OPS-01');
DECLARE @OPS03 INT = (SELECT ProcedureId FROM dbo.OpsProcedure WHERE Code = N'OPS-03');
DECLARE @OPS05 INT = (SELECT ProcedureId FROM dbo.OpsProcedure WHERE Code = N'OPS-05');

-- Kế hoạch chuyến, Checklist trước khởi hành
INSERT INTO dbo.OpsTemplate (TemplateKey, ProcedureId, TemplateNo, Name, TemplateType, State, FileName)
VALUES
    (N'T1',   @OPS01, N'FM-OPS-01', N'Kế hoạch chuyến (Form)',           N'Form',      N'Approved',  N'FM-OPS-01.docx'),
    (N'T2',   @OPS01, N'CL-OPS-01', N'Checklist trước khởi hành',        N'Checklist', N'Submitted', N'CL-OPS-01.xlsx'),
    (N'T6ihr',@OPS05, N'SOF 05-04-01', N'SOF 05-04-01 TANK PREPARATION PLAN', N'Checklist', N'Draft', N'SOF 05-04-01 TANK PREPARATION PLAN.xls'),
    (N'Ts3ks',@OPS05, N'SOF 05-04-02', N'KẾ HOẠCH RỬA KÉT HÀNG - TANK CLEANING PLAN', N'Form', N'Draft', N'SOF 05-04-02 KẾ HOẠCH RỬA KÉT HÀNG - TANK CLEANING PLAN.docx');

------------------------------------------------------------
-- Submission & Approval demo (2 mẫu, giống giao diện)
------------------------------------------------------------

DECLARE @Ship15 INT = (SELECT UnitId FROM dbo.Unit WHERE UnitName = N'Tàu Petrolimex 15');
DECLARE @Tech   INT = (SELECT UnitId FROM dbo.Unit WHERE UnitName = N'Phòng Kỹ thuật');
DECLARE @QHSE   INT = (SELECT UnitId FROM dbo.Unit WHERE UnitName = N'Phòng An toàn/QHSE');
DECLARE @Board  INT = (SELECT UnitId FROM dbo.Unit WHERE UnitName = N'Ban Giám đốc');

DECLARE @TplPlan INT = (SELECT TemplateId FROM dbo.OpsTemplate WHERE TemplateKey = N'T1');
DECLARE @TplValve INT = NULL; -- demo đơn giản, chỉ gắn vào OPS-03 nếu cần mở rộng sau

-- SUB-BUNDI4: Kế hoạch chuyến (Form)
INSERT INTO dbo.OpsSubmission
    (SubmissionCode, ProcedureId, TemplateId, UnitId, SenderUserId, SentAt, DueDate, State, Note)
VALUES
    (N'SUB-BUNDI4', @OPS01, @TplPlan, @Ship15, @A, '2025-03-21T10:00', '2025-03-25', N'Submitted', N'Chuyến 03/2025');

DECLARE @SubBUNDI4 INT = SCOPE_IDENTITY();

INSERT INTO dbo.OpsSubmissionRecipient (SubmissionId, UnitId)
VALUES
    (@SubBUNDI4, @Tech),
    (@SubBUNDI4, @QHSE);

INSERT INTO dbo.OpsSubmissionFile (SubmissionId, FileName, UploadedAt)
VALUES
    (@SubBUNDI4, N'KeHoachChuyen_PLX15_202503.docx', '2025-03-21T10:00');

-- Hàng phê duyệt tương ứng
INSERT INTO dbo.OpsApproval
    (ApprovalCode, SubmissionId, Kind, Name, ProcedureId, SenderUnitId, RequestedAt, State)
VALUES
    (N'APR-SUB-BUNDI4', @SubBUNDI4, N'Biểu mẫu đã nộp', N'Kế hoạch chuyến (Form)', @OPS01, @Ship15, '2025-03-21T10:00', N'Submitted');

-- SUB-4YGHSE: Biên bản kiểm tra van hàng theo quý (rút gọn)
INSERT INTO dbo.OpsSubmission
    (SubmissionCode, ProcedureId, TemplateId, UnitId, SenderUserId, SentAt, DueDate, State, Note)
VALUES
    (N'SUB-4YGHSE', @OPS03, NULL, @Ship15, @A, '2025-10-29T02:11:24', '2025-10-29', N'Submitted', N'Biên bản kiểm tra van hàng theo quý');

DECLARE @Sub4YGHSE INT = SCOPE_IDENTITY();

INSERT INTO dbo.OpsSubmissionRecipient (SubmissionId, UnitId)
VALUES (@Sub4YGHSE, @Board);

INSERT INTO dbo.OpsSubmissionFile (SubmissionId, FileName, UploadedAt)
VALUES (@Sub4YGHSE, N'SOF 03-06-01A RECORD OF CARGO VALVES INSPECTION.doc', '2025-10-29T02:11:24');

INSERT INTO dbo.OpsApproval
    (ApprovalCode, SubmissionId, Kind, Name, ProcedureId, SenderUnitId, RequestedAt, State)
VALUES
    (N'APR-SUB-4YGHSE', @Sub4YGHSE, N'Biểu mẫu đã nộp', 
     N'Biên bản kiểm tra van hàng theo quý - Quarterly Record of Cargo valves inspection',
     @OPS03, @Ship15, '2025-10-29T02:11:24', N'Submitted');

------------------------------------------------------------
-- Nhật ký demo
------------------------------------------------------------

INSERT INTO dbo.OpsAuditLog (LoggedAt, UserId, Action, Target, Detail)
VALUES
    ('2025-10-28T13:47:33', @A, N'Init',        N'Demo', N'Khởi tạo dữ liệu'),
    ('2025-10-28T13:49:25', @E, N'Edit',        N'OPS-01', N'Thông tin chung'),
    ('2025-10-28T14:04:16', @E, N'Edit',        N'OPS-02', N'Thông tin chung'),
    ('2025-10-28T14:05:56', @E, N'Edit',        N'OPS-03', N'Thông tin chung'),
    ('2025-10-28T14:26:31', @E, N'Add template',N'Biên bản mẫu hàng - Cargo sample record', NULL),
    ('2025-10-28T14:32:46', @E, N'Add template',N'Biên bản kiểm tra van hàng theo quý - Quarterly Record of Cargo valves inspection', NULL),
    ('2025-10-29T01:54:45', @A, N'Create',      N'OPS-04', N'HỆ THỐNG KIỂM SOÁT HƠI HÀNG CARGO VAPOUR EMISSIONS CONTROL - VEC SYSTEMS'),
    ('2025-10-29T02:04:17', @A, N'Create',      N'OPS-05', N'VỆ SINH KÉT - TANK CLEANING'),
    ('2025-10-29T02:11:24', @A, N'Submit form', N'SUB-4YGHSE', N'Biên bản kiểm tra van hàng theo quý'),
    ('2025-10-29T02:11:24', @A, N'Submit form', N'SUB-BUNDI4', N'Kế hoạch chuyến (Form)');
GO

PRINT N'Đã tạo xong cấu trúc CSDL và dữ liệu demo cho modul Quản lý Khai thác tàu.';
