# Hướng dẫn xử lý Database Schema Conflict

## Vấn đề

Bạn đã có database `SSMS_KhaiThacTau` với schema từ file `Database/SSMS_Ops_DBV2.sql` với các bảng:
- `AppUser` (UserId, UserName, LoginName, Email, Phone, IsActive, CreatedAt)
- `Unit` (UnitId, UnitName, UnitCode, UnitType, IsActive, SortOrder)

Nhưng Entity Framework Core models cần thêm các trường:
- `AppUser`: FullName, Role, Position, UnitId, PasswordHash, LastLoginAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
- `Unit`: Description, ParentUnitId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted

## Giải pháp: Chạy SQL Script để thêm các cột mới

### Bước 1: Tạo file SQL để thêm các cột mới

Tạo file `Database/AddExtendedFields.sql`:

```sql
USE SSMS_KhaiThacTau;
GO

-- Thêm các cột mới vào bảng AppUser
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'FullName')
    ALTER TABLE dbo.AppUser ADD FullName NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'Role')
    ALTER TABLE dbo.AppUser ADD Role NVARCHAR(50) NOT NULL DEFAULT 'User';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'Position')
    ALTER TABLE dbo.AppUser ADD Position NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'UnitId')
    ALTER TABLE dbo.AppUser ADD UnitId INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'PasswordHash')
    ALTER TABLE dbo.AppUser ADD PasswordHash NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'LastLoginAt')
    ALTER TABLE dbo.AppUser ADD LastLoginAt DATETIME2(0) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'UpdatedAt')
    ALTER TABLE dbo.AppUser ADD UpdatedAt DATETIME2(0) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'CreatedBy')
    ALTER TABLE dbo.AppUser ADD CreatedBy NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'UpdatedBy')
    ALTER TABLE dbo.AppUser ADD UpdatedBy NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.AppUser') AND name = 'IsDeleted')
    ALTER TABLE dbo.AppUser ADD IsDeleted BIT NOT NULL DEFAULT 0;

GO

-- Thêm các cột mới vào bảng Unit
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'Description')
    ALTER TABLE dbo.Unit ADD Description NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'ParentUnitId')
    ALTER TABLE dbo.Unit ADD ParentUnitId INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'CreatedAt')
    ALTER TABLE dbo.Unit ADD CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME();

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'UpdatedAt')
    ALTER TABLE dbo.Unit ADD UpdatedAt DATETIME2(0) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'CreatedBy')
    ALTER TABLE dbo.Unit ADD CreatedBy NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'UpdatedBy')
    ALTER TABLE dbo.Unit ADD UpdatedBy NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Unit') AND name = 'IsDeleted')
    ALTER TABLE dbo.Unit ADD IsDeleted BIT NOT NULL DEFAULT 0;

GO

-- Thêm Foreign Key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AppUser_Unit_UnitId')
    ALTER TABLE dbo.AppUser 
    ADD CONSTRAINT FK_AppUser_Unit_UnitId 
    FOREIGN KEY (UnitId) REFERENCES dbo.Unit(UnitId);

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Unit_Unit_ParentUnitId')
    ALTER TABLE dbo.Unit 
    ADD CONSTRAINT FK_Unit_Unit_ParentUnitId 
    FOREIGN KEY (ParentUnitId) REFERENCES dbo.Unit(UnitId);

GO

-- Update existing data với giá trị mặc định
UPDATE dbo.AppUser 
SET FullName = UserName 
WHERE FullName IS NULL;

-- Gán UnitId cho users hiện có (giả sử tất cả thuộc Unit đầu tiên)
DECLARE @FirstUnitId INT = (SELECT TOP 1 UnitId FROM dbo.Unit ORDER BY UnitId);
UPDATE dbo.AppUser 
SET UnitId = @FirstUnitId 
WHERE UnitId IS NULL;

GO

-- Tạo bảng __EFMigrationsHistory nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

PRINT 'Đã thêm các cột mới vào database thành công!';
```

### Bước 2: Chạy SQL Script

```bash
# Sử dụng sqlcmd
sqlcmd -S localhost,1433 -U sa -P MyStrong@Password123 -i Database/AddExtendedFields.sql

# Hoặc sử dụng Azure Data Studio / SQL Server Management Studio
```

### Bước 3: Tạo Migration Baseline

Sau khi đã thêm các cột qua SQL, tạo migration baseline:

```bash
cd backend
dotnet ef migrations add InitialBaseline --project src/SSMS.Infrastructure --startup-project src/SSMS.API --context AppDbContext
```

### Bước 4: Mark migration as applied (không chạy thực tế)

```bash
# Thêm migration vào history table mà không chạy
dotnet ef database update --project src/SSMS.Infrastructure --startup-project src/SSMS.API --context AppDbContext
```

## Lưu ý

- Sau khi chạy SQL script, EF Core sẽ sync với database hiện có
- Các migration tiếp theo sẽ hoạt động bình thường
- Nên backup database trước khi chạy script

## Alternative: Scaffold từ Database hiện có

Nếu muốn, bạn có thể scaffold entities từ database hiện có:

```bash
dotnet ef dbcontext scaffold "Server=localhost,1433;Database=SSMS_KhaiThacTau;User Id=sa;Password=MyStrong@Password123;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context AppDbContext --force
```

Sau đó merge với code hiện tại.
