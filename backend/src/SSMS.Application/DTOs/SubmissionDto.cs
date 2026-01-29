namespace SSMS.Application.DTOs;

/// <summary>
/// DTO cho biểu mẫu đã nộp
/// </summary>
public class SubmissionDto
{
    public int Id { get; set; }
    public int ProcedureId { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public int SubmittedByUserId { get; set; }
    public string SubmittedByUserName { get; set; } = string.Empty;
    public DateTime? RecalledAt { get; set; }
    public string? RecallReason { get; set; }
    public List<SubmissionFileDto> Files { get; set; } = new();
    public List<SubmissionRecipientDto> Recipients { get; set; } = new();
    public bool CanRecall { get; set; }
    public int? ApproverUserId { get; set; }
    public string ApproverUserName { get; set; } = string.Empty;
    public bool CanApprove { get; set; }
}

/// <summary>
/// DTO cho tạo mới biểu mẫu
/// </summary>
public class SubmissionCreateDto
{
    public int ProcedureId { get; set; }
    public int? TemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public List<int>? RecipientUserIds { get; set; }
}

/// <summary>
/// DTO cho file đính kèm
/// </summary>
public class SubmissionFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// DTO cho người nhận
/// </summary>
public class SubmissionRecipientDto
{
    public int SubmissionId { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public int? RecipientUserId { get; set; }
    public string RecipientUserName { get; set; } = string.Empty;
    public string? RecipientRole { get; set; }
    public string RecipientType { get; set; } = string.Empty;
}

/// <summary>
/// DTO cho thu hồi biểu mẫu
/// </summary>
public class RecallDto
{
    public string Reason { get; set; } = string.Empty;
}
