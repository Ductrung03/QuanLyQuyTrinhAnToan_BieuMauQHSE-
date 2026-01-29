namespace SSMS.Core.Entities;

/// <summary>
/// Biểu mẫu đã nộp
/// </summary>
public class OpsSubmission : BaseEntity
{
    /// <summary>
    /// Mã biểu mẫu (unique, format: SUB-YYYYMMDD-NNN)
    /// </summary>
    public string SubmissionCode { get; set; } = string.Empty;

    /// <summary>
    /// ID quy trình
    /// </summary>
    public int ProcedureId { get; set; }

    /// <summary>
    /// Quy trình
    /// </summary>
    public OpsProcedure Procedure { get; set; } = null!;

    /// <summary>
    /// ID template (nếu có)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Template
    /// </summary>
    public OpsTemplate? Template { get; set; }

    /// <summary>
    /// ID người nộp
    /// </summary>
    public int SubmittedByUserId { get; set; }

    /// <summary>
    /// Người nộp
    /// </summary>
    public AppUser SubmittedByUser { get; set; } = null!;

    /// <summary>
    /// Thời điểm nộp
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Trạng thái (Submitted, Approved, Rejected, Recalled)
    /// </summary>
    public string Status { get; set; } = "Submitted";

    /// <summary>
    /// Tiêu đề
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Nội dung/Ghi chú
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Thời điểm thu hồi (nếu có)
    /// </summary>
    public DateTime? RecalledAt { get; set; }

    /// <summary>
    /// Lý do thu hồi
    /// </summary>
    public string? RecallReason { get; set; }

    /// <summary>
    /// Files đính kèm
    /// </summary>
    public ICollection<OpsSubmissionFile> Files { get; set; } = new List<OpsSubmissionFile>();

    /// <summary>
    /// Người nhận (CC)
    /// </summary>
    public ICollection<OpsSubmissionRecipient> Recipients { get; set; } = new List<OpsSubmissionRecipient>();
}
