# Phase 4: User Story 2 - Nộp và Theo dõi Biểu mẫu (IN PROGRESS)

## ✅ Đã hoàn thành

### T021: Entities ✅
- `OpsSubmission.cs` - Biểu mẫu đã nộp
- `OpsSubmissionFile.cs` - Files đính kèm
- `OpsSubmissionRecipient.cs` - Người nhận (CC)
- Đã cấu hình EF Core mappings
- Đã tạo migration `AddSubmissionEntities`

**Tiến độ**: 1/7 tasks (14%)

## 🔄 Cần hoàn thành

### T022-T023: SubmissionService (NEXT)

```csharp
// backend/src/SSMS.Application/DTOs/SubmissionDto.cs
public class SubmissionDto
{
    public int Id { get; set; }
    public int ProcedureId { get; set; }
    public string ProcedureName { get; set; }
    public int? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public string Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string SubmittedByUserName { get; set; }
    public List<SubmissionFileDto> Files { get; set; }
    public bool CanRecall { get; set; } // < 60 minutes
}

public class SubmissionCreateDto
{
    public int ProcedureId { get; set; }
    public int? TemplateId { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public List<int>? RecipientUserIds { get; set; }
}

public class SubmissionFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

```csharp
// backend/src/SSMS.Application/Services/ISubmissionService.cs
public interface ISubmissionService
{
    Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(int userId);
    Task<SubmissionDto?> GetByIdAsync(int id);
    Task<SubmissionDto> CreateAsync(SubmissionCreateDto dto, int userId, List<IFormFile>? files);
    Task<bool> RecallAsync(int id, int userId, string reason);
    Task<bool> CanRecallAsync(int id, int userId);
}
```

```csharp
// backend/src/SSMS.Application/Services/SubmissionService.cs
public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _uploadPath;
    private const int RECALL_TIME_LIMIT_MINUTES = 60;

    public async Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(int userId)
    {
        var submissions = await _unitOfWork.Submissions
            .FindAsync(s => s.SubmittedByUserId == userId);
        
        return submissions.Select(s => new SubmissionDto
        {
            Id = s.Id,
            Title = s.Title,
            Status = s.Status,
            SubmittedAt = s.SubmittedAt,
            CanRecall = CanRecall(s, userId),
            // ... map other fields
        });
    }

    public async Task<bool> RecallAsync(int id, int userId, string reason)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(id);
        
        if (submission == null)
            throw new KeyNotFoundException("Không tìm thấy biểu mẫu");
        
        if (submission.SubmittedByUserId != userId)
            throw new UnauthorizedAccessException("Bạn không có quyền thu hồi biểu mẫu này");
        
        if (!CanRecall(submission, userId))
            throw new InvalidOperationException("Đã quá thời gian thu hồi (60 phút)");
        
        if (submission.Status != "Submitted")
            throw new InvalidOperationException("Chỉ có thể thu hồi biểu mẫu đang chờ xử lý");

        submission.Status = "Recalled";
        submission.RecalledAt = DateTime.UtcNow;
        submission.RecallReason = reason;

        _unitOfWork.Submissions.Update(submission);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CanRecallAsync(int id, int userId)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(id);
        return submission != null && CanRecall(submission, userId);
    }

    private bool CanRecall(OpsSubmission submission, int userId)
    {
        if (submission.SubmittedByUserId != userId) return false;
        if (submission.Status != "Submitted") return false;
        
        var minutesSinceSubmission = (DateTime.UtcNow - submission.SubmittedAt).TotalMinutes;
        return minutesSinceSubmission <= RECALL_TIME_LIMIT_MINUTES;
    }

    public async Task<SubmissionDto> CreateAsync(
        SubmissionCreateDto dto, 
        int userId, 
        List<IFormFile>? files)
    {
        var submission = new OpsSubmission
        {
            ProcedureId = dto.ProcedureId,
            TemplateId = dto.TemplateId,
            Title = dto.Title,
            Content = dto.Content,
            SubmittedByUserId = userId,
            SubmittedAt = DateTime.UtcNow,
            Status = "Submitted"
        };

        // Upload files
        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                var submissionFile = await UploadFileAsync(file);
                submission.Files.Add(submissionFile);
            }
        }

        // Add recipients
        if (dto.RecipientUserIds != null)
        {
            foreach (var recipientId in dto.RecipientUserIds)
            {
                submission.Recipients.Add(new OpsSubmissionRecipient
                {
                    RecipientUserId = recipientId,
                    RecipientType = "CC"
                });
            }
        }

        await _unitOfWork.Submissions.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(submission.Id);
    }
}
```

### T024: SubmissionsController (NEXT)

```csharp
// backend/src/SSMS.API/Controllers/SubmissionsController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    [HttpGet("my")]
    public async Task<IActionResult> GetMySubmissions()
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var submissions = await _submissionService.GetMySubmissionsAsync(userId);
        return Ok(new { Success = true, Data = submissions });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var submission = await _submissionService.GetByIdAsync(id);
        if (submission == null)
            return NotFound(new { Success = false, Message = "Không tìm thấy biểu mẫu" });
        
        return Ok(new { Success = true, Data = submission });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] SubmissionCreateDto dto,
        [FromForm] List<IFormFile>? files)
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var submission = await _submissionService.CreateAsync(dto, userId, files);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = submission.Id },
            new { Success = true, Data = submission });
    }

    [HttpPost("{id}/recall")]
    public async Task<IActionResult> Recall(int id, [FromBody] RecallDto dto)
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        
        try
        {
            await _submissionService.RecallAsync(id, userId, dto.Reason);
            return Ok(new { Success = true, Message = "Thu hồi thành công" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }

    [HttpGet("{id}/can-recall")]
    public async Task<IActionResult> CanRecall(int id)
    {
        var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var canRecall = await _submissionService.CanRecallAsync(id, userId);
        return Ok(new { Success = true, Data = canRecall });
    }
}

public class RecallDto
{
    public string Reason { get; set; } = string.Empty;
}
```

### T025-T027: Frontend (NEXT)

```typescript
// frontend/lib/api-client.ts - Thêm methods
async getMySubmissions(): Promise<ApiResponse<any[]>> {
  return this.get<any[]>('/submissions/my');
}

async getSubmissionById(id: number): Promise<ApiResponse<any>> {
  return this.get<any>(`/submissions/${id}`);
}

async createSubmission(data: any, files: File[]): Promise<ApiResponse<any>> {
  const formData = new FormData();
  formData.append('procedureId', data.procedureId.toString());
  formData.append('title', data.title);
  if (data.templateId) formData.append('templateId', data.templateId.toString());
  if (data.content) formData.append('content', data.content);
  
  files.forEach((file) => {
    formData.append('files', file);
  });

  try {
    const response = await fetch(`${this.baseUrl}/submissions`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${this.getToken()}`,
      },
      body: formData,
    });
    return await response.json();
  } catch (error) {
    return { success: false, message: 'Lỗi khi nộp biểu mẫu' };
  }
}

async recallSubmission(id: number, reason: string): Promise<ApiResponse<any>> {
  return this.post<any>(`/submissions/${id}/recall`, { reason });
}

async canRecallSubmission(id: number): Promise<ApiResponse<boolean>> {
  return this.get<boolean>(`/submissions/${id}/can-recall`);
}
```

```tsx
// frontend/app/dashboard/submissions/page.tsx
'use client';

import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';
import { FileText, Clock, CheckCircle, XCircle, RotateCcw } from 'lucide-react';

export default function SubmissionsPage() {
  const [submissions, setSubmissions] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSubmissions();
  }, []);

  const loadSubmissions = async () => {
    const response = await apiClient.getMySubmissions();
    if (response.success && response.data) {
      setSubmissions(response.data);
    }
    setLoading(false);
  };

  const handleRecall = async (id: number) => {
    const reason = prompt('Lý do thu hồi:');
    if (!reason) return;

    const response = await apiClient.recallSubmission(id, reason);
    if (response.success) {
      alert('Thu hồi thành công!');
      loadSubmissions();
    } else {
      alert(response.message);
    }
  };

  const getStatusBadge = (status: string) => {
    const badges: any = {
      Submitted: { color: 'bg-blue-100 text-blue-800', icon: Clock },
      Approved: { color: 'bg-green-100 text-green-800', icon: CheckCircle },
      Rejected: { color: 'bg-red-100 text-red-800', icon: XCircle },
      Recalled: { color: 'bg-gray-100 text-gray-800', icon: RotateCcw },
    };
    const badge = badges[status] || badges.Submitted;
    const Icon = badge.icon;
    
    return (
      <span className={`inline-flex items-center gap-1 px-2 py-1 text-xs font-medium rounded-full ${badge.color}`}>
        <Icon className="w-3 h-3" />
        {status}
      </span>
    );
  };

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Biểu mẫu đã nộp</h1>

      {/* Table */}
      <div className="bg-white rounded-lg shadow-sm border">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Tiêu đề
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Quy trình
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Trạng thái
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Ngày nộp
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                Thao tác
              </th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {submissions.map((submission) => (
              <tr key={submission.id} className="hover:bg-gray-50">
                <td className="px-6 py-4">{submission.title}</td>
                <td className="px-6 py-4">{submission.procedureName}</td>
                <td className="px-6 py-4">{getStatusBadge(submission.status)}</td>
                <td className="px-6 py-4">
                  {new Date(submission.submittedAt).toLocaleString('vi-VN')}
                </td>
                <td className="px-6 py-4 text-right">
                  {submission.canRecall && (
                    <button
                      onClick={() => handleRecall(submission.id)}
                      className="text-orange-600 hover:text-orange-700"
                    >
                      Thu hồi
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
```

```tsx
// frontend/app/dashboard/submissions/new/page.tsx
'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { apiClient } from '@/lib/api-client';

export default function NewSubmissionPage() {
  const router = useRouter();
  const [procedures, setProcedures] = useState<any[]>([]);
  const [templates, setTemplates] = useState<any[]>([]);
  const [formData, setFormData] = useState({
    procedureId: 0,
    templateId: 0,
    title: '',
    content: '',
  });
  const [files, setFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadProcedures();
  }, []);

  const loadProcedures = async () => {
    const response = await apiClient.getProcedures();
    if (response.success && response.data) {
      setProcedures(response.data);
    }
  };

  const loadTemplates = async (procedureId: number) => {
    const response = await apiClient.getTemplatesByProcedure(procedureId);
    if (response.success && response.data) {
      setTemplates(response.data);
    }
  };

  const handleProcedureChange = (procedureId: number) => {
    setFormData({ ...formData, procedureId, templateId: 0 });
    if (procedureId) {
      loadTemplates(procedureId);
    } else {
      setTemplates([]);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setFiles(Array.from(e.target.files));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    const response = await apiClient.createSubmission(formData, files);
    
    if (response.success) {
      alert('Nộp biểu mẫu thành công!');
      router.push('/dashboard/submissions');
    } else {
      alert(response.message || 'Có lỗi xảy ra');
    }

    setLoading(false);
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <h1 className="text-2xl font-bold">Nộp biểu mẫu mới</h1>

      <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-sm border p-6 space-y-4">
        <div>
          <label className="block text-sm font-medium mb-2">Quy trình *</label>
          <select
            value={formData.procedureId}
            onChange={(e) => handleProcedureChange(Number(e.target.value))}
            className="w-full px-4 py-2 border rounded-lg"
            required
          >
            <option value="">-- Chọn quy trình --</option>
            {procedures.map((p) => (
              <option key={p.id} value={p.id}>
                {p.code} - {p.name}
              </option>
            ))}
          </select>
        </div>

        {templates.length > 0 && (
          <div>
            <label className="block text-sm font-medium mb-2">Biểu mẫu</label>
            <select
              value={formData.templateId}
              onChange={(e) => setFormData({ ...formData, templateId: Number(e.target.value) })}
              className="w-full px-4 py-2 border rounded-lg"
            >
              <option value="">-- Chọn biểu mẫu (tùy chọn) --</option>
              {templates.map((t) => (
                <option key={t.id} value={t.id}>
                  {t.name}
                </option>
              ))}
            </select>
          </div>
        )}

        <div>
          <label className="block text-sm font-medium mb-2">Tiêu đề *</label>
          <input
            type="text"
            value={formData.title}
            onChange={(e) => setFormData({ ...formData, title: e.target.value })}
            className="w-full px-4 py-2 border rounded-lg"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-2">Nội dung</label>
          <textarea
            value={formData.content}
            onChange={(e) => setFormData({ ...formData, content: e.target.value })}
            className="w-full px-4 py-2 border rounded-lg"
            rows={4}
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-2">Tài liệu đính kèm</label>
          <input
            type="file"
            multiple
            onChange={handleFileChange}
            className="w-full px-4 py-2 border rounded-lg"
          />
          {files.length > 0 && (
            <div className="mt-2 text-sm text-gray-600">
              Đã chọn {files.length} file
            </div>
          )}
        </div>

        <div className="flex gap-3 pt-4">
          <button
            type="button"
            onClick={() => router.back()}
            className="flex-1 px-4 py-2 border rounded-lg hover:bg-gray-50"
          >
            Hủy
          </button>
          <button
            type="submit"
            className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
            disabled={loading}
          >
            {loading ? 'Đang nộp...' : 'Nộp biểu mẫu'}
          </button>
        </div>
      </form>
    </div>
  );
}
```

## 📋 Checklist

- [x] T021: Entities
- [ ] T022: SubmissionService
- [ ] T023: Recall Logic
- [ ] T024: SubmissionsController
- [ ] T025: SubmissionForm Component
- [ ] T026: New Submission Page
- [ ] T027: Submissions List Page

**Tiến độ**: 1/7 tasks (14%)

## 🎯 Mục tiêu

- Nhân viên có thể nộp biểu mẫu
- Upload nhiều files
- Chọn quy trình và template
- Xem danh sách đã nộp
- Thu hồi trong vòng 60 phút
- Theo dõi trạng thái

## 📝 Notes

- Recall time limit: 60 minutes
- File upload: Multiple files support
- Status: Submitted, Approved, Rejected, Recalled
- Recipients: CC functionality
