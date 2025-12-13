# Phase 3: User Story 1 - Quản lý Quy trình ✅ HOÀN THÀNH

## 🎉 Tổng kết

**Phase 3 Backend đã hoàn thành 100%!**

### ✅ Đã hoàn thành tất cả tasks

| Task | Mô tả | Trạng thái |
|------|-------|-----------|
| **T013** | Entities (OpsProcedure, OpsProcedureDocument, OpsTemplate) | ✅ |
| **T014** | DTOs (ProcedureDto, TemplateDto, DocumentDto) | ✅ |
| **T015** | ProcedureService & TemplateService | ✅ |
| **T016** | ProceduresController | ✅ |
| **T017** | TemplatesController | ✅ |

**Tiến độ**: 5/5 tasks (100%) ✅

---

## 📁 Files đã tạo

### Entities (T013)
```
backend/src/SSMS.Core/Entities/
├── OpsProcedure.cs          - Quy trình vận hành
├── OpsProcedureDocument.cs  - Tài liệu đính kèm
└── OpsTemplate.cs           - Biểu mẫu/Checklist
```

### DTOs (T014)
```
backend/src/SSMS.Application/DTOs/
├── ProcedureDto.cs          - ProcedureDto, ProcedureCreateDto, ProcedureUpdateDto, ProcedureListDto
├── ProcedureDocumentDto.cs  - ProcedureDocumentDto, DocumentUploadDto
└── TemplateDto.cs           - TemplateDto, TemplateCreateDto, TemplateUpdateDto
```

### Services (T015)
```
backend/src/SSMS.Application/Services/
├── IProcedureService.cs     - Interface cho Procedure service
├── ProcedureService.cs      - Implementation với CRUD + file upload
├── ITemplateService.cs      - Interface cho Template service
└── TemplateService.cs       - Implementation với CRUD + file upload
```

### Controllers (T016-T017)
```
backend/src/SSMS.API/Controllers/
├── ProceduresController.cs  - API endpoints cho Procedures
└── TemplatesController.cs   - API endpoints cho Templates
```

### Infrastructure Updates
```
backend/src/SSMS.Core/Interfaces/
└── IUnitOfWork.cs           - Thêm Procedures, ProcedureDocuments, Templates repositories

backend/src/SSMS.Infrastructure/Data/
├── AppDbContext.cs          - Thêm DbSets và configurations
└── Repositories/
    └── UnitOfWork.cs        - Implement repositories mới
```

---

## 🔌 API Endpoints

### Procedures API (`/api/procedures`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/procedures` | Lấy danh sách quy trình | ✅ |
| GET | `/api/procedures/{id}` | Lấy chi tiết quy trình | ✅ |
| POST | `/api/procedures` | Tạo quy trình mới | Manager/Admin |
| PUT | `/api/procedures/{id}` | Cập nhật quy trình | Manager/Admin |
| DELETE | `/api/procedures/{id}` | Xóa quy trình | Admin Only |
| POST | `/api/procedures/{id}/documents` | Upload tài liệu | Manager/Admin |
| DELETE | `/api/procedures/documents/{documentId}` | Xóa tài liệu | Manager/Admin |

### Templates API (`/api/templates`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/templates` | Lấy danh sách templates | ✅ |
| GET | `/api/templates/procedure/{procedureId}` | Lấy templates theo procedure | ✅ |
| GET | `/api/templates/{id}` | Lấy chi tiết template | ✅ |
| POST | `/api/templates` | Tạo template mới | Manager/Admin |
| PUT | `/api/templates/{id}` | Cập nhật template | Manager/Admin |
| DELETE | `/api/templates/{id}` | Xóa template | Admin Only |
| POST | `/api/templates/{id}/upload` | Upload file template | Manager/Admin |

---

## 🎯 Tính năng đã implement

### ProcedureService
- ✅ GetAllAsync() - Lấy danh sách với thông tin rút gọn
- ✅ GetByIdAsync() - Lấy chi tiết đầy đủ với documents & templates
- ✅ CreateAsync() - Tạo mới với validation (check duplicate Code)
- ✅ UpdateAsync() - Cập nhật thông tin
- ✅ DeleteAsync() - Soft delete
- ✅ UploadDocumentAsync() - Upload file với validation (extension, size)
- ✅ DeleteDocumentAsync() - Xóa file vật lý và record

### TemplateService
- ✅ GetAllAsync() - Lấy tất cả templates
- ✅ GetByProcedureIdAsync() - Lấy templates theo procedure
- ✅ GetByIdAsync() - Lấy chi tiết template
- ✅ CreateAsync() - Tạo mới với optional file upload
- ✅ UpdateAsync() - Cập nhật thông tin
- ✅ DeleteAsync() - Soft delete + xóa file
- ✅ UploadFileAsync() - Upload/Replace file template

### File Upload Features
- ✅ Validation: Extension (.pdf, .doc, .docx, .xls, .xlsx)
- ✅ Validation: Max size 20MB
- ✅ Unique filename generation (GUID)
- ✅ Organized folder structure (`uploads/procedures/`, `uploads/templates/`)
- ✅ Auto create directories
- ✅ Physical file deletion on record delete

---

## 🔐 Authorization

### Policies Applied
- **Authorize**: Tất cả endpoints yêu cầu authentication
- **ManagerOrAdmin**: Create, Update, Upload operations
- **AdminOnly**: Delete operations

### Unit-based Authorization
- Đã sẵn sàng để implement (có UnitId trong Procedure)
- Có thể filter procedures theo Unit của user

---

## 🗄️ Database

### Tables Created
```sql
OpsProcedure
├── ProcedureId (PK)
├── Code (Unique)
├── Name
├── Version
├── State (Draft/Submitted/Approved/Rejected)
├── Description
├── OwnerUserId (FK -> AppUser)
├── AuthorUserId (FK -> AppUser)
├── ApproverUserId (FK -> AppUser)
├── CreatedDate
├── ReleasedDate
└── BaseEntity fields (CreatedAt, UpdatedAt, IsDeleted...)

OpsProcedureDocument
├── ProcedureDocId (PK)
├── ProcedureId (FK -> OpsProcedure, CASCADE)
├── DocVersion
├── FileName
├── FilePath
├── FileSize
├── ContentType
├── UploadedAt
└── BaseEntity fields

OpsTemplate
├── TemplateId (PK)
├── ProcedureId (FK -> OpsProcedure, CASCADE)
├── TemplateKey
├── TemplateNo
├── Name
├── TemplateType (Form/Checklist)
├── State
├── FileName
├── FilePath
├── FileSize
├── ContentType
├── IsActive
└── BaseEntity fields
```

### Migration
- ✅ Migration `AddProcedureEntities` đã được tạo
- ✅ Đã mark as applied (bảng đã tồn tại từ SQL script)

---

## 🧪 Testing

### Cách test API

1. **Start Backend**:
```bash
cd backend
dotnet run --project src/SSMS.API
```

2. **Login để lấy token**:
```bash
POST http://localhost:5000/api/auth/login
{
  "userId": 1  # Admin user
}
```

3. **Test Procedures API**:
```bash
# Get all procedures
GET http://localhost:5000/api/procedures
Authorization: Bearer {token}

# Create procedure
POST http://localhost:5000/api/procedures
Authorization: Bearer {token}
{
  "code": "OPS-01",
  "name": "Quy trình an toàn lao động",
  "version": "1.0",
  "description": "Mô tả quy trình",
  "ownerUserId": 2,
  "authorUserId": 1
}

# Upload document
POST http://localhost:5000/api/procedures/1/documents
Authorization: Bearer {token}
Content-Type: multipart/form-data
file: [select file]
docVersion: "1.0"
```

4. **Test Templates API**:
```bash
# Create template
POST http://localhost:5000/api/templates
Authorization: Bearer {token}
Content-Type: multipart/form-data
procedureId: 1
name: "Biểu mẫu kiểm tra an toàn"
templateType: "Form"
templateNo: "FM-OPS-01"
file: [select file]
```

---

## 📊 Response Format

Tất cả API đều trả về format chuẩn:

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Thành công" // optional
}
```

### Error Response
```json
{
  "success": false,
  "message": "Mô tả lỗi"
}
```

---

## 🔜 Bước tiếp theo

### Frontend Tasks (T018-T020)
Cần implement frontend cho Phase 3:

1. **T018**: UI Components
   - `ProcedureList.tsx` - Danh sách quy trình với filters
   - `ProcedureForm.tsx` - Form tạo/sửa quy trình
   - `ProcedureDetail.tsx` - Chi tiết quy trình
   - `DocumentUpload.tsx` - Component upload tài liệu
   - `TemplateList.tsx` - Danh sách biểu mẫu

2. **T019**: API Integration
   - Tích hợp API procedures
   - Tích hợp API templates
   - Error handling
   - Loading states

3. **T020**: Advanced Features
   - File download
   - Preview documents
   - Drag & drop upload
   - Bulk operations

### Phase 4: User Story 2 - Nộp Biểu mẫu
Sau khi hoàn thành Frontend Phase 3, tiếp tục với:
- Submission entities
- Submission workflow
- Approval process

---

## ✅ Checklist hoàn thành

- [x] T013: Entities
- [x] T014: DTOs
- [x] T015: Services
- [x] T016: ProceduresController
- [x] T017: TemplatesController
- [x] Database migrations
- [x] Service registration
- [x] Build successful
- [ ] Frontend implementation (Next phase)
- [ ] Integration testing (Next phase)
- [ ] End-to-end testing (Next phase)

---

## 🎯 Summary

**Phase 3 Backend: HOÀN THÀNH 100%** ✅

Hệ thống đã có đầy đủ:
- ✅ Data models
- ✅ Business logic
- ✅ API endpoints
- ✅ File upload handling
- ✅ Authorization
- ✅ Error handling
- ✅ Logging

**Sẵn sàng cho Frontend integration!** 🚀
