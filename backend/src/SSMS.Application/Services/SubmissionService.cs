using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service cho quản lý biểu mẫu
/// </summary>
public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _uploadPath;
    private const int RECALL_TIME_LIMIT_MINUTES = 60;

    public SubmissionService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _uploadPath = configuration["FileUpload:SubmissionsPath"] ?? "uploads/submissions";
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(int userId)
    {
        var submissions = await _unitOfWork.Submissions
            .FindAsync(s => s.SubmittedByUserId == userId);
            
        // Order by SubmittedAt DESC manually as FindAsync returns IEnumerable or wesort in memory
        submissions = submissions.OrderByDescending(s => s.SubmittedAt);

        var result = new List<SubmissionDto>();
        foreach (var sub in submissions)
        {
            result.Add(await MapToDtoAsync(sub, userId));
        }

        return result;
    }

    public async Task<SubmissionDto?> GetByIdAsync(int id)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(id);

        if (submission == null)
            return null;

        return await MapToDtoAsync(submission, submission.SubmittedByUserId);
    }

    public async Task<SubmissionDto> CreateAsync(
        SubmissionCreateDto dto,
        int userId,
        List<IFormFile>? files)
    {
        // Validate procedure exists
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(dto.ProcedureId);
        if (procedure == null)
            throw new KeyNotFoundException("Không tìm thấy quy trình");

        // Validate template if provided
        if (dto.TemplateId.HasValue)
        {
            var template = await _unitOfWork.Templates.GetByIdAsync(dto.TemplateId.Value);
            if (template == null)
                throw new KeyNotFoundException("Không tìm thấy biểu mẫu");
        }

        // Generate SubmissionCode
        var submissionCode = await GenerateSubmissionCodeAsync();

        var submission = new OpsSubmission
        {
            SubmissionCode = submissionCode,
            ProcedureId = dto.ProcedureId,
            TemplateId = dto.TemplateId,
            Title = dto.Title,
            Content = dto.Content,
            SubmittedByUserId = userId,
            SubmittedAt = DateTime.UtcNow,
            Status = "Submitted"
        };

        // Note: Files and Recipients are collections. 
        // Since we are not using Include/Context directly, specific repositories might be needed 
        // OR we just add to the entity if the repository Attach mechanism works. 
        // But Repository<T>.AddAsync just does _dbSet.AddAsync(entity).
        // EF Core usually tracks connected entities.
        
        // Upload files
        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                var submissionFile = await UploadFileAsync(file);
                // We need to add to the File repository explicitly if tracking isn't enough, 
                // but usually adding to the parent collection works if the parent is added.
                submission.Files.Add(submissionFile);
            }
        }

        // Add recipients
        if (dto.RecipientUserIds != null && dto.RecipientUserIds.Count > 0)
        {
            foreach (var recipientId in dto.RecipientUserIds)
            {
                // Get user to retrieve UnitId
                var recipientUser = await _unitOfWork.Users.GetByIdAsync(recipientId);
                var unitId = recipientUser?.UnitId ?? 1; // Default to unit 1 if no unit
                
                submission.Recipients.Add(new OpsSubmissionRecipient
                {
                    RecipientUserId = recipientId,
                    UnitId = unitId,
                    RecipientType = "CC"
                });
            }
        }

        await _unitOfWork.Submissions.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(submission.Id))!;
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

    #region Private Methods

    private bool CanRecall(OpsSubmission submission, int userId)
    {
        if (submission.SubmittedByUserId != userId) return false;
        if (submission.Status != "Submitted") return false;

        var minutesSinceSubmission = (DateTime.UtcNow - submission.SubmittedAt).TotalMinutes;
        return minutesSinceSubmission <= RECALL_TIME_LIMIT_MINUTES;
    }

    private async Task<string> GenerateSubmissionCodeAsync()
    {
        // Format: SUB-YYYYMMDD-NNN
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var dateStr = today.ToString("yyyyMMdd");
        
        // Count submissions for today using date range (SQL Server compatible)
        var todaySubmissions = await _unitOfWork.Submissions.FindAsync(
            s => s.SubmittedAt >= today && s.SubmittedAt < tomorrow
        );
        var count = todaySubmissions.Count() + 1;
        
        return $"SUB-{dateStr}-{count:D3}";
    }

    private async Task<OpsSubmissionFile> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File không hợp lệ");

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new OpsSubmissionFile
        {
            FileName = file.FileName,
            FilePath = filePath,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };
    }

    private async Task<SubmissionDto> MapToDtoAsync(OpsSubmission submission, int currentUserId)
    {
        // Load related data manually
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(submission.ProcedureId);
        var template = submission.TemplateId.HasValue 
            ? await _unitOfWork.Templates.GetByIdAsync(submission.TemplateId.Value) 
            : null;
        var submittedByUser = await _unitOfWork.Users.GetByIdAsync(submission.SubmittedByUserId);
        var approverUser = procedure?.ApproverUserId.HasValue == true
            ? await _unitOfWork.Users.GetByIdAsync(procedure.ApproverUserId.Value)
            : null;
        
        var files = await _unitOfWork.SubmissionFiles.FindAsync(f => f.SubmissionId == submission.Id);
        var recipients = await _unitOfWork.SubmissionRecipients.FindAsync(r => r.SubmissionId == submission.Id);

        var recipientDtos = new List<SubmissionRecipientDto>();
        foreach (var r in recipients)
        {
            var user = r.RecipientUserId.HasValue
                ? await _unitOfWork.Users.GetByIdAsync(r.RecipientUserId.Value)
                : null;
            var unit = await _unitOfWork.Units.GetByIdAsync(r.UnitId);
            recipientDtos.Add(new SubmissionRecipientDto
            {
                SubmissionId = r.SubmissionId,
                UnitId = r.UnitId,
                UnitName = unit?.Name ?? "",
                RecipientUserId = r.RecipientUserId,
                RecipientUserName = user?.FullName ?? "",
                RecipientRole = r.RecipientRole,
                RecipientType = r.RecipientType
            });
        }

        return new SubmissionDto
        {
            Id = submission.Id,
            ProcedureId = submission.ProcedureId,
            ProcedureName = procedure?.Name ?? "",
            ProcedureCode = procedure?.Code ?? "",
            TemplateId = submission.TemplateId,
            TemplateName = template?.Name,
            Title = submission.Title,
            Content = submission.Content,
            Status = submission.Status,
            SubmittedAt = submission.SubmittedAt,
            SubmittedByUserId = submission.SubmittedByUserId,
            SubmittedByUserName = submittedByUser?.FullName ?? "",
            RecalledAt = submission.RecalledAt,
            RecallReason = submission.RecallReason,
            Files = files.Select(f => new SubmissionFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                FilePath = f.FilePath,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            }).ToList(),
            Recipients = recipientDtos,
            CanRecall = CanRecall(submission, currentUserId),
            ApproverUserId = procedure?.ApproverUserId,
            ApproverUserName = approverUser?.FullName ?? "",
            CanApprove = (procedure?.ApproverUserId == currentUserId) && (submission.Status == "Submitted")
        };
    }

    #endregion
}
