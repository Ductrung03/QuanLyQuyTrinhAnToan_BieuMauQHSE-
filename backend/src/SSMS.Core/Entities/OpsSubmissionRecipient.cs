namespace SSMS.Core.Entities;

/// <summary>
/// Người nhận (CC) của submission - Composite key: (SubmissionId, UnitId)
/// </summary>
public class OpsSubmissionRecipient
{
    /// <summary>
    /// ID submission (Part of composite key)
    /// </summary>
    public int SubmissionId { get; set; }

    /// <summary>
    /// Submission navigation
    /// </summary>
    public OpsSubmission Submission { get; set; } = null!;

    /// <summary>
    /// ID Unit (Part of composite key)
    /// </summary>
    public int UnitId { get; set; }

    /// <summary>
    /// Unit navigation
    /// </summary>
    public Unit Unit { get; set; } = null!;

    /// <summary>
    /// ID người nhận (nullable)
    /// </summary>
    public int? RecipientUserId { get; set; }

    /// <summary>
    /// Người nhận navigation
    /// </summary>
    public AppUser? RecipientUser { get; set; }

    /// <summary>
    /// Role của người nhận
    /// </summary>
    public string? RecipientRole { get; set; }

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

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}
