namespace SSMS.Core.Entities;

/// <summary>
/// File đính kèm của submission
/// </summary>
public class OpsSubmissionFile : BaseEntity
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
    /// Tên file gốc
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn lưu trữ
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Kích thước file (bytes)
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Content type
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Thời điểm upload
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
