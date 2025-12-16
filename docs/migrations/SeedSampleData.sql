-- Seed Sample Data for SSMS QHSE
-- Run this to populate the database with realistic test data

USE SSMS_KhaiThacTau;
GO

-- Cleanup existing data to avoid duplicates
DELETE FROM OpsApproval;
DELETE FROM OpsSubmission;
DELETE FROM OpsTemplate;
DELETE FROM OpsProcedure;
-- Can optionally delete AuditLog or keep it
-- DELETE FROM OpsAuditLog;

-- 1. Insert Procedures (Quy trình)
-- 1. Insert Procedures (Quy trình)
INSERT INTO OpsProcedure (Code, Name, Description, State, CreatedAt, CreatedBy, IsDeleted)
VALUES 
('OPS-001', N'Quy trình Làm việc trên cao', N'Quy định an toàn khi thực hiện các công việc ở độ cao trên 2m', 'Approved', GETUTCDATE(), N'Admin', 0),
('OPS-002', N'Quy trình Vào không gian kín', N'Quy trình đảm bảo an toàn khi ra vào hầm hàng, két nước, không gian hạn chế', 'Approved', GETUTCDATE(), N'Admin', 0),
('OPS-003', N'Quy trình Đánh giá rủi ro', N'Hướng dẫn thực hiện đánh giá rủi ro trước khi thực hiện công việc', 'Approved', GETUTCDATE(), N'Admin', 0),
('OPS-004', N'Quy trình Ứng cứu khẩn cấp', N'Các bước xử lý tình huống khẩn cấp trên tàu', 'Draft', GETUTCDATE(), N'Admin', 0),
('OPS-005', N'Quy trình Quản lý rác thải', N'Hướng dẫn phân loại và xử lý rác thải theo MARPOL', 'Submitted', GETUTCDATE(), N'Admin', 0);

-- 2. Insert Templates (Biểu mẫu) linked to Procedures
DECLARE @Proc1Id INT = (SELECT ProcedureId FROM OpsProcedure WHERE Code = 'OPS-001');
DECLARE @Proc2Id INT = (SELECT ProcedureId FROM OpsProcedure WHERE Code = 'OPS-002');
DECLARE @Proc3Id INT = (SELECT ProcedureId FROM OpsProcedure WHERE Code = 'OPS-003');

INSERT INTO OpsTemplate (ProcedureId, TemplateNo, TemplateName, Name, TemplateType, CreatedAt, CreatedBy, IsDeleted, State)
VALUES 
(@Proc1Id, 'FM-OPS-001-01', N'Giấy phép làm việc trên cao', N'Giấy phép làm việc trên cao', N'Permit', GETUTCDATE(), N'Admin', 0, 'Active'),
(@Proc1Id, 'FM-OPS-001-02', N'Bảng kiểm an toàn làm việc trên cao', N'Bảng kiểm an toàn làm việc trên cao', N'Checklist', GETUTCDATE(), N'Admin', 0, 'Active'),
(@Proc2Id, 'FM-OPS-002-01', N'Giấy phép vào không gian kín', N'Giấy phép vào không gian kín', N'Permit', GETUTCDATE(), N'Admin', 0, 'Active'),
(@Proc3Id, 'FM-OPS-003-01', N'Phiếu đánh giá rủi ro công việc', N'Phiếu đánh giá rủi ro công việc', N'Form', GETUTCDATE(), N'Admin', 0, 'Active');

-- 3. Insert Submissions (Nộp biểu mẫu)
DECLARE @Temp1Id INT = (SELECT TemplateId FROM OpsTemplate WHERE TemplateNo = 'FM-OPS-001-01');
DECLARE @Temp2Id INT = (SELECT TemplateId FROM OpsTemplate WHERE TemplateNo = 'FM-OPS-002-01');
DECLARE @User1Id INT = 1; -- Nguyen Van A (Admin)
DECLARE @User2Id INT = 2; -- Tran Thi B (Manager)
DECLARE @UnitId INT = (SELECT TOP 1 UnitId FROM Unit); -- Get first available Unit

INSERT INTO OpsSubmission (ProcedureId, TemplateId, SubmittedByUserId, SenderUserId, SubmittedAt, Status, State, Title, Content, CreatedAt, IsDeleted, SubmissionCode, UnitId)
VALUES 
-- Submission 1: Pending
(@Proc1Id, @Temp1Id, @User2Id, @User2Id, DATEADD(HOUR, -2, GETUTCDATE()), 'Submitted', 'Submitted', N'Xin cấp phép sơn cột buồm', N'{"location": "Cột buồm mũi", "height": "15m"}', DATEADD(HOUR, -2, GETUTCDATE()), 0, 'SUB-001', @UnitId),
-- Submission 2: Approved
(@Proc2Id, @Temp2Id, @User1Id, @User1Id, DATEADD(DAY, -1, GETUTCDATE()), 'Approved', 'Approved', N'Vệ sinh két nước ngọt số 1', N'{"tank": "FW Tank 1", "gas_check": "Safe"}', DATEADD(DAY, -1, GETUTCDATE()), 0, 'SUB-002', @UnitId),
-- Submission 3: Rejected
(@Proc1Id, @Temp1Id, @User2Id, @User2Id, DATEADD(DAY, -2, GETUTCDATE()), 'Rejected', 'Rejected', N'Sửa chữa đèn tín hiệu', N'{"weather": "Bad"}', DATEADD(DAY, -2, GETUTCDATE()), 0, 'SUB-003', @UnitId),
-- Submission 4: Draft
(@Proc2Id, @Temp2Id, @User1Id, @User1Id, DATEADD(HOUR, -5, GETUTCDATE()), 'Draft', 'Draft', N'Kiểm tra hầm hàng số 2', N'{}', DATEADD(HOUR, -5, GETUTCDATE()), 0, 'SUB-004', @UnitId);

-- 4. Insert Approvals (Lịch sử phê duyệt)
DECLARE @Sub2Id INT = (SELECT SubmissionId FROM OpsSubmission WHERE Title = N'Vệ sinh két nước ngọt số 1');
DECLARE @Sub3Id INT = (SELECT SubmissionId FROM OpsSubmission WHERE Title = N'Sửa chữa đèn tín hiệu');

-- Note: OpsApproval might require ApprovalCode or other fields, setting arbitrary code
INSERT INTO OpsApproval (SubmissionId, Action, ActionDate, ApproverUserId, Note, CreatedAt, IsDeleted, ApprovalCode, State, Name, Kind)
VALUES 
(@Sub2Id, 'Approve', DATEADD(DAY, -1, GETUTCDATE()), @User2Id, N'Đã kiểm tra khí, đủ điều kiện an toàn.', DATEADD(DAY, -1, GETUTCDATE()), 0, 'APP-01', 'Completed', N'Phê duyệt cấp 1', 'Sequential'),
(@Sub3Id, 'Reject', DATEADD(DAY, -2, GETUTCDATE()), @User1Id, N'Thời tiết xấu, sóng lớn không được làm việc trên cao.', DATEADD(DAY, -2, GETUTCDATE()), 0, 'APP-02', 'Rejected', N'Phê duyệt cấp 1', 'Sequential');

-- 5. Additional Audit Logs
INSERT INTO OpsAuditLog (UserId, UserName, Action, TargetType, TargetId, TargetName, Detail, ActionTime, CreatedAt, IsDeleted)
VALUES 
(@User1Id, 'Admin', 'Approve', 'Submission', @Sub2Id, N'Vệ sinh két nước ngọt số 1', N'Phê duyệt giấy phép', DATEADD(DAY, -1, GETUTCDATE()), GETUTCDATE(), 0),
(@User2Id, 'Manager', 'Reject', 'Submission', @Sub3Id, N'Sửa chữa đèn tín hiệu', N'Từ chối do thời tiết', DATEADD(DAY, -2, GETUTCDATE()), GETUTCDATE(), 0),
(@User2Id, 'Manager', 'Submit', 'Submission', NULL, N'Xin cấp phép sơn cột buồm', N'Nộp biểu mẫu mới', DATEADD(HOUR, -2, GETUTCDATE()), GETUTCDATE(), 0);

PRINT 'Sample data seeded successfully!';
GO
