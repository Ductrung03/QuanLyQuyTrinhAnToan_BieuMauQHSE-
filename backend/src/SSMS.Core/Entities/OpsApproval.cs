using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMS.Core.Entities;

/// <summary>
/// Lịch sử phê duyệt biểu mẫu
/// </summary>
[Table("OpsApproval")]
public class OpsApproval : BaseEntity
{
    /// <summary>
    /// ID của biểu mẫu được phê duyệt
    /// </summary>
    public int SubmissionId { get; set; }

    [ForeignKey(nameof(SubmissionId))]
    public OpsSubmission Submission { get; set; } = null!;

    /// <summary>
    /// Người thực hiện phê duyệt
    /// </summary>
    public int ApproverUserId { get; set; }

    [ForeignKey(nameof(ApproverUserId))]
    public AppUser Approver { get; set; } = null!;

    /// <summary>
    /// Hành động: "Approve" hoặc "Reject"
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Ghi chú / Lý do từ chối
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Thời điểm thực hiện
    /// </summary>
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
}
