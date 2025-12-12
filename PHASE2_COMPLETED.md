# Phase 2: Foundational - Hoàn thành ✅

## Tổng quan
Phase 2 đã hoàn thành việc xây dựng nền tảng cho hệ thống SSMS QHSE, bao gồm:
- Database schema với EF Core
- Mock Authentication
- Unit-based Authorization
- Base Repository & UnitOfWork pattern
- Frontend Layout & Login

## Các Task đã hoàn thành

### Backend

#### ✅ T006: Entity Models
- `BaseEntity.cs` - Base class cho tất cả entities
- `Unit.cs` - Đơn vị tổ chức (phòng ban, tàu, chi nhánh)
- `AppUser.cs` - Người dùng với UnitId

#### ✅ T007: Database Context
- `AppDbContext.cs` - EF Core DbContext
- Cấu hình relationships, indexes
- Auto-update timestamps
- Soft delete query filters

#### ✅ T008: Migrations & Seed Data
- Migration `InitialCreate` đã tạo
- Seed data: 5 Units, 8 Users với các vai trò khác nhau
- Database `SSMS_KhaiThacTau` đã được tạo thành công

#### ✅ T009: Mock Authentication
- `MockAuthService.cs` - Login bằng dropdown chọn user
- `AuthModels.cs` - DTOs cho authentication
- `AuthController.cs` - API endpoints (/auth/users, /auth/login, /auth/me)
- JWT token generation

#### ✅ T010: Repository Pattern
- `IRepository<T>` - Generic repository interface
- `Repository<T>` - Implementation với EF Core
- `IUnitOfWork` - Unit of Work interface
- `UnitOfWork` - Transaction management

#### ✅ Authorization Policies
- `UnitRequirement` - Phân quyền theo đơn vị
- `RoleRequirement` - Phân quyền theo vai trò
- Policies: `SameUnit`, `AdminOnly`, `ManagerOrAdmin`

### Frontend

#### ✅ T011: Dashboard Layout
- `app/dashboard/layout.tsx` - Layout với Sidebar & Header
- Navigation menu với role-based filtering
- User info display
- Responsive sidebar toggle

#### ✅ T012: Login Page
- `app/login/page.tsx` - Mock login với dropdown
- API integration
- Token & user data storage
- Error handling

#### ✅ Additional Components
- `lib/api-client.ts` - API client với authentication
- `app/dashboard/page.tsx` - Dashboard overview
- `app/page.tsx` - Root redirect

## Cấu trúc Database

### Tables Created
1. **Units** - Đơn vị tổ chức
   - Hỗ trợ cấu trúc phân cấp (ParentUnitId)
   - Các loại: Headquarters, Ship, Department

2. **AppUsers** - Người dùng
   - Liên kết với Unit (UnitId)
   - Roles: Admin, Manager, User
   - Soft delete support

### Seed Data
**Units:**
- Trụ sở chính (HQ)
- Tàu Hải Phòng 01, 02 (Ships)
- Phòng QHSE, Phòng Khai thác (Departments)

**Users:**
- admin (Admin - Trụ sở chính)
- qhse.manager (Manager - Phòng QHSE)
- ops.manager (Manager - Phòng Khai thác)
- ship001.captain, ship001.officer (User - Tàu 01)
- ship002.captain, ship002.officer (User - Tàu 02)
- qhse.staff (User - Phòng QHSE)

## API Endpoints

### Authentication
```
GET  /api/auth/users     - Lấy danh sách users (cho dropdown)
POST /api/auth/login     - Login (body: { userId: number })
GET  /api/auth/me        - Lấy thông tin user hiện tại [Authorized]
```

### Health Check
```
GET  /health             - Health check endpoint
```

## Cách chạy

### Backend
```bash
cd backend
dotnet restore
dotnet ef database update --project src/SSMS.Infrastructure --startup-project src/SSMS.API
dotnet run --project src/SSMS.API
```
API sẽ chạy tại: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### Frontend
```bash
cd frontend
npm install
npm run dev
```
Frontend sẽ chạy tại: `http://localhost:3000`

## Test Authentication

1. Mở `http://localhost:3000`
2. Trang sẽ redirect đến `/login`
3. Chọn một user từ dropdown (ví dụ: "Quản trị viên hệ thống (Admin)")
4. Click "Đăng nhập"
5. Sẽ redirect đến `/dashboard`

## Authorization Policies

### SameUnit Policy
- User chỉ có thể truy cập dữ liệu của đơn vị mình
- Admin có thể override (access tất cả)

### AdminOnly Policy
- Chỉ Admin mới có quyền

### ManagerOrAdmin Policy
- Manager và Admin có quyền

## Các bước tiếp theo (Phase 3)

Phase 3 sẽ triển khai **User Story 1: Quản lý Quy trình**
- Backend: Entities, Services, Controllers cho Procedures, Templates
- Frontend: UI components cho CRUD quy trình
- File upload handling
- Template management

## Notes

- Mock Authentication được sử dụng cho development, không yêu cầu password
- JWT token expires sau 480 phút (8 giờ)
- Connection string sử dụng SQL Server authentication
- Soft delete được implement cho tất cả entities
- Timestamps tự động cập nhật (CreatedAt, UpdatedAt)

## Checkpoint ✅

Hệ thống đã có thể:
- ✅ Đăng nhập với Mock Authentication
- ✅ Nhận diện quyền hạn theo Role và Unit
- ✅ Kết nối CSDL thành công
- ✅ Hiển thị Dashboard với navigation
- ✅ API endpoints hoạt động
- ✅ Authorization policies được cấu hình

**Phase 2 hoàn thành! Sẵn sàng cho Phase 3.**
