# Phase 3: User Story 1 - Quản lý Quy trình (IN PROGRESS)

## ✅ Đã hoàn thành

### T013: Entities ✅
- `OpsProcedure.cs` - Quy trình vận hành
- `OpsProcedureDocument.cs` - Tài liệu đính kèm
- `OpsTemplate.cs` - Biểu mẫu/Checklist
- Đã cấu hình EF Core mappings trong `AppDbContext`
- Đã tạo migration `AddProcedureEntities`
- Database đã có sẵn các bảng từ SQL script ban đầu

## 🔄 Đang thực hiện

### T014: DTOs (NEXT)
Cần tạo các DTOs trong `backend/src/SSMS.Application/DTOs/`:
- `ProcedureDto.cs`
- `ProcedureCreateDto.cs`
- `ProcedureUpdateDto.cs`
- `ProcedureDocumentDto.cs`
- `TemplateDto.cs`
- `TemplateCreateDto.cs`

### T015: ProcedureService (NEXT)
Cần implement trong `backend/src/SSMS.Application/Services/`:
- `IProcedureService.cs` - Interface
- `ProcedureService.cs` - Implementation
  - GetAllAsync()
  - GetByIdAsync(id)
  - CreateAsync(dto)
  - UpdateAsync(id, dto)
  - DeleteAsync(id)
  - UploadDocumentAsync(procedureId, file)
  - DeleteDocumentAsync(documentId)

### T016: ProceduresController (NEXT)
Cần tạo trong `backend/src/SSMS.API/Controllers/`:
- `ProceduresController.cs`
  - GET /api/procedures
  - GET /api/procedures/{id}
  - POST /api/procedures
  - PUT /api/procedures/{id}
  - DELETE /api/procedures/{id}
  - POST /api/procedures/{id}/documents
  - DELETE /api/procedures/documents/{documentId}

### T017: TemplatesController (NEXT)
Cần tạo trong `backend/src/SSMS.API/Controllers/`:
- `TemplatesController.cs`
  - GET /api/templates
  - GET /api/templates/{id}
  - POST /api/templates
  - PUT /api/templates/{id}
  - DELETE /api/templates/{id}

### T018-T020: Frontend (NEXT)
Cần tạo trong `frontend/`:
- `src/components/business/procedures/ProcedureList.tsx`
- `src/components/business/procedures/ProcedureForm.tsx`
- `src/components/business/procedures/DocumentUpload.tsx`
- `src/app/(dashboard)/procedures/page.tsx`
- `src/app/(dashboard)/procedures/[id]/page.tsx`

## 📝 Hướng dẫn tiếp tục

### Bước 1: Tạo DTOs

```csharp
// backend/src/SSMS.Application/DTOs/ProcedureDto.cs
namespace SSMS.Application.DTOs;

public class ProcedureDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string State { get; set; } = "Draft";
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ReleasedDate { get; set; }
    
    // Owner info
    public int? OwnerUserId { get; set; }
    public string? OwnerUserName { get; set; }
    
    // Author info
    public int? AuthorUserId { get; set; }
    public string? AuthorUserName { get; set; }
    
    // Approver info
    public int? ApproverUserId { get; set; }
    public string? ApproverUserName { get; set; }
    
    // Collections
    public List<ProcedureDocumentDto> Documents { get; set; } = new();
    public List<TemplateDto> Templates { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ProcedureCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Description { get; set; }
    public int? OwnerUserId { get; set; }
    public int? AuthorUserId { get; set; }
}

public class ProcedureUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string State { get; set; } = "Draft";
    public string? Description { get; set; }
    public int? OwnerUserId { get; set; }
    public int? ApproverUserId { get; set; }
    public DateTime? ReleasedDate { get; set; }
}
```

### Bước 2: Implement Service

```csharp
// backend/src/SSMS.Application/Services/IProcedureService.cs
public interface IProcedureService
{
    Task<IEnumerable<ProcedureDto>> GetAllAsync();
    Task<ProcedureDto?> GetByIdAsync(int id);
    Task<ProcedureDto> CreateAsync(ProcedureCreateDto dto);
    Task<ProcedureDto> UpdateAsync(int id, ProcedureUpdateDto dto);
    Task DeleteAsync(int id);
    Task<ProcedureDocumentDto> UploadDocumentAsync(int procedureId, IFormFile file);
    Task DeleteDocumentAsync(int documentId);
}
```

### Bước 3: Implement Controller

```csharp
// backend/src/SSMS.API/Controllers/ProceduresController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureService _procedureService;
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var procedures = await _procedureService.GetAllAsync();
        return Ok(new { Success = true, Data = procedures });
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var procedure = await _procedureService.GetByIdAsync(id);
        if (procedure == null)
            return NotFound(new { Success = false, Message = "Không tìm thấy quy trình" });
        return Ok(new { Success = true, Data = procedure });
    }
    
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] ProcedureCreateDto dto)
    {
        var procedure = await _procedureService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = procedure.Id }, 
            new { Success = true, Data = procedure });
    }
    
    // ... other endpoints
}
```

### Bước 4: Frontend Components

```typescript
// frontend/src/app/(dashboard)/procedures/page.tsx
'use client';

import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';

export default function ProceduresPage() {
  const [procedures, setProcedures] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadProcedures();
  }, []);

  const loadProcedures = async () => {
    const response = await apiClient.get('/procedures');
    if (response.success) {
      setProcedures(response.data);
    }
    setLoading(false);
  };

  return (
    <div>
      <h1>Quản lý Quy trình</h1>
      {/* Table, filters, actions */}
    </div>
  );
}
```

## 🎯 Mục tiêu Phase 3

- [ ] T013: Entities ✅ DONE
- [ ] T014: DTOs
- [ ] T015: ProcedureService
- [ ] T016: ProceduresController
- [ ] T017: TemplatesController
- [ ] T018: UI Components
- [ ] T019: API Integration (List & Create)
- [ ] T020: Upload & Template Management

## 📊 Tiến độ

**Hoàn thành**: 1/8 tasks (12.5%)
**Còn lại**: 7 tasks

## 🔗 Dependencies

- UnitOfWork cần thêm repositories cho OpsProcedure, OpsProcedureDocument, OpsTemplate
- File upload service cần implement
- Authorization policies cần áp dụng cho endpoints

## 📚 Tài liệu tham khảo

- Database schema: `Database/SSMS_Ops_DBV2.sql`
- Spec: `specs/001-qhse-process-management/spec.md`
- Tasks: `specs/001-qhse-process-management/tasks.md`
