using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _0002_SeedRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [Role] ([Name], [Code], [Description], [IsSystemRole], [CreatedAt], [IsDeleted])
                VALUES
                    (N'Quản trị viên hệ thống', 'ADMIN', N'Toàn quyền quản trị hệ thống', 1, SYSUTCDATETIME(), 0),
                    (N'Quản lý', 'MANAGER', N'Quản lý phòng ban/bộ phận', 1, SYSUTCDATETIME(), 0),
                    (N'Thuyền trưởng', 'CAPTAIN', N'Thuyền trưởng tàu khai thác', 1, SYSUTCDATETIME(), 0),
                    (N'Cán bộ An toàn', 'SAFETY_OFFICER', N'Cán bộ An toàn QHSE', 1, SYSUTCDATETIME(), 0),
                    (N'Người dùng', 'USER', N'Người dùng cơ bản', 1, SYSUTCDATETIME(), 0);

                INSERT INTO [Permission] ([Name], [Code], [Module], [Description], [CreatedAt], [IsDeleted])
                VALUES
                    (N'Quản lý hệ thống', 'system.manage', 'System', N'Toàn quyền quản lý hệ thống', SYSUTCDATETIME(), 0),
                    (N'Xem người dùng', 'user.view', 'System', N'Xem danh sách người dùng', SYSUTCDATETIME(), 0),
                    (N'Tạo người dùng', 'user.create', 'System', N'Tạo người dùng mới', SYSUTCDATETIME(), 0),
                    (N'Sửa người dùng', 'user.edit', 'System', N'Chỉnh sửa thông tin người dùng', SYSUTCDATETIME(), 0),
                    (N'Xóa người dùng', 'user.delete', 'System', N'Xóa người dùng', SYSUTCDATETIME(), 0),
                    (N'Xem quy trình', 'proc.view', 'Procedure', N'Xem danh sách quy trình', SYSUTCDATETIME(), 0),
                    (N'Tạo quy trình', 'proc.create', 'Procedure', N'Tạo quy trình mới', SYSUTCDATETIME(), 0),
                    (N'Sửa quy trình', 'proc.edit', 'Procedure', N'Chỉnh sửa quy trình', SYSUTCDATETIME(), 0),
                    (N'Xóa quy trình', 'proc.delete', 'Procedure', N'Xóa quy trình', SYSUTCDATETIME(), 0),
                    (N'Duyệt quy trình', 'proc.approve', 'Procedure', N'Phê duyệt quy trình', SYSUTCDATETIME(), 0),
                    (N'Xem mẫu biểu', 'tmpl.view', 'Template', N'Xem danh sách mẫu biểu', SYSUTCDATETIME(), 0),
                    (N'Tạo mẫu biểu', 'tmpl.create', 'Template', N'Tạo mẫu biểu mới', SYSUTCDATETIME(), 0),
                    (N'Sửa mẫu biểu', 'tmpl.edit', 'Template', N'Chỉnh sửa mẫu biểu', SYSUTCDATETIME(), 0),
                    (N'Xóa mẫu biểu', 'tmpl.delete', 'Template', N'Xóa mẫu biểu', SYSUTCDATETIME(), 0),
                    (N'Duyệt mẫu biểu', 'tmpl.approve', 'Template', N'Phê duyệt mẫu biểu', SYSUTCDATETIME(), 0),
                    (N'Xem hồ sơ nộp', 'sub.view', 'Submission', N'Xem danh sách hồ sơ nộp', SYSUTCDATETIME(), 0),
                    (N'Tạo hồ sơ nộp', 'sub.create', 'Submission', N'Tạo hồ sơ nộp mới', SYSUTCDATETIME(), 0),
                    (N'Sửa hồ sơ nộp', 'sub.edit', 'Submission', N'Chỉnh sửa hồ sơ nộp', SYSUTCDATETIME(), 0),
                    (N'Xóa hồ sơ nộp', 'sub.delete', 'Submission', N'Xóa hồ sơ nộp', SYSUTCDATETIME(), 0),
                    (N'Duyệt hồ sơ nộp', 'sub.approve', 'Submission', N'Phê duyệt hồ sơ nộp', SYSUTCDATETIME(), 0),
                    (N'Xem hồ sơ vận hành', 'ops.view', 'Operations', N'Xem danh sách hồ sơ vận hành', SYSUTCDATETIME(), 0),
                    (N'Tạo hồ sơ vận hành', 'ops.create', 'Operations', N'Tạo hồ sơ vận hành mới', SYSUTCDATETIME(), 0),
                    (N'Sửa hồ sơ vận hành', 'ops.edit', 'Operations', N'Chỉnh sửa hồ sơ vận hành', SYSUTCDATETIME(), 0),
                    (N'Xóa hồ sơ vận hành', 'ops.delete', 'Operations', N'Xóa hồ sơ vận hành', SYSUTCDATETIME(), 0),
                    (N'Duyệt hồ sơ vận hành', 'ops.approve', 'Operations', N'Phê duyệt hồ sơ vận hành', SYSUTCDATETIME(), 0),
                    (N'Xem đơn vị', 'unit.view', 'Unit', N'Xem danh sách đơn vị', SYSUTCDATETIME(), 0),
                    (N'Tạo đơn vị', 'unit.create', 'Unit', N'Tạo đơn vị mới', SYSUTCDATETIME(), 0),
                    (N'Sửa đơn vị', 'unit.edit', 'Unit', N'Chỉnh sửa đơn vị', SYSUTCDATETIME(), 0),
                    (N'Xóa đơn vị', 'unit.delete', 'Unit', N'Xóa đơn vị', SYSUTCDATETIME(), 0),
                    (N'Quản lý đơn vị', 'unit.manage', 'Unit', N'Toàn quyền quản lý đơn vị', SYSUTCDATETIME(), 0),
                    (N'Xem nhật ký', 'audit.view', 'Audit', N'Xem nhật ký hệ thống', SYSUTCDATETIME(), 0),
                    (N'Xuất nhật ký', 'audit.export', 'Audit', N'Xuất báo cáo nhật ký', SYSUTCDATETIME(), 0),
                    (N'Xóa nhật ký', 'audit.delete', 'Audit', N'Xóa nhật ký', SYSUTCDATETIME(), 0),
                    (N'Xem nhật ký vận hành', 'audit.ops.view', 'Audit', N'Xem nhật ký vận hành tàu', SYSUTCDATETIME(), 0),
                    (N'Xuất nhật ký vận hành', 'audit.ops.export', 'Audit', N'Xuất báo cáo nhật ký vận hành', SYSUTCDATETIME(), 0),
                    (N'Xem báo cáo', 'report.view', 'Report', N'Xem các báo cáo', SYSUTCDATETIME(), 0),
                    (N'Tạo báo cáo', 'report.create', 'Report', N'Tạo báo cáo mới', SYSUTCDATETIME(), 0),
                    (N'Xuất báo cáo', 'report.export', 'Report', N'Xuất báo cáo ra file', SYSUTCDATETIME(), 0),
                    (N'Xóa báo cáo', 'report.delete', 'Report', N'Xóa báo cáo', SYSUTCDATETIME(), 0),
                    (N'Duyệt báo cáo', 'report.approve', 'Report', N'Phê duyệt báo cáo', SYSUTCDATETIME(), 0);

                INSERT INTO [RolePermission] ([RoleId], [PermissionId], [CreatedAt])
                SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
                FROM [Role] r
                CROSS JOIN [Permission] p
                WHERE r.Code = 'ADMIN';

                INSERT INTO [RolePermission] ([RoleId], [PermissionId], [CreatedAt])
                SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
                FROM [Role] r
                CROSS JOIN [Permission] p
                WHERE r.Code = 'MANAGER'
                  AND p.Code NOT IN ('system.manage', 'user.create', 'user.delete', 'proc.delete', 'tmpl.delete', 'sub.delete', 'ops.delete', 'audit.delete');

                INSERT INTO [RolePermission] ([RoleId], [PermissionId], [CreatedAt])
                SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
                FROM [Role] r
                CROSS JOIN [Permission] p
                WHERE r.Code = 'CAPTAIN'
                  AND p.Code IN (
                    'proc.view', 'tmpl.view', 'sub.view', 'sub.create', 'sub.edit',
                    'ops.view', 'ops.create', 'ops.edit', 'ops.approve',
                    'audit.ops.view', 'audit.ops.export',
                    'report.view', 'report.create', 'report.export', 'unit.view'
                  );

                INSERT INTO [RolePermission] ([RoleId], [PermissionId], [CreatedAt])
                SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
                FROM [Role] r
                CROSS JOIN [Permission] p
                WHERE r.Code = 'SAFETY_OFFICER'
                  AND p.Code IN (
                    'proc.view', 'proc.create', 'proc.edit', 'proc.approve',
                    'tmpl.view', 'tmpl.create', 'tmpl.edit', 'tmpl.approve',
                    'sub.view', 'sub.create', 'sub.edit', 'sub.approve',
                    'ops.view', 'ops.approve',
                    'audit.view', 'audit.export', 'audit.ops.view', 'audit.ops.export',
                    'report.view', 'unit.view'
                  );

                INSERT INTO [RolePermission] ([RoleId], [PermissionId], [CreatedAt])
                SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
                FROM [Role] r
                CROSS JOIN [Permission] p
                WHERE r.Code = 'USER'
                  AND p.Code IN (
                    'proc.view', 'tmpl.view',
                    'sub.view', 'sub.create',
                    'ops.view', 'ops.create',
                    'audit.ops.view',
                    'report.view', 'report.create', 'unit.view'
                  );

                UPDATE u
                SET u.RoleId = r.RoleId
                FROM [AppUser] u
                INNER JOIN [Role] r ON u.Role = r.Code COLLATE SQL_Latin1_General_CP1_CI_AS
                WHERE u.RoleId IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [RolePermission];
                DELETE FROM [Permission];
                DELETE FROM [Role];
                UPDATE [AppUser] SET RoleId = NULL;
            ");
        }
    }
}
