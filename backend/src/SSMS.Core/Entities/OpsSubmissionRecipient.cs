namespace SSMS.Core.Entities;

/// <summary>
/// Người nhận (CC) của submission
/// </summary>
public class OpsSubmissionRecipient : BaseEntity
{
    /// <summary>
    /// ID submission
    /// </summary>
    public int SubmissionId { get; set; }

    /// <summary>
    /// Submission
    /// </summary>
    public OpsSubmission Submission { get; set; } = null!;

    /// <summary>
    /// ID người nhận
    /// </summary>
    public int RecipientUserId { get; set; }

    /// <summary>
    /// Người nhận
    /// </summary>
    public AppUser RecipientUser { get; set; } = null!;

    /// <summary>
    /// Loại (To, CC)
    /// </summary>
    public string RecipientType { get; set; } = "CC";

    /// <summary>
    /// Đã đọc chưa
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Thời điểm đọc
    /// </summary>
    public DateTime? ReadAt { get; set; }
}
