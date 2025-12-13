namespace SSMS.Core.Entities;

/// <summary>
/// Tài liệu đính kèm cho quy trình (DOCX/PDF...)
/// </summary>
public class OpsProcedureDocument : BaseEntity
{
    /// <summary>
    /// ID quy trình
    /// </summary>
    public int ProcedureId { get; set; }

    /// <summary>
    /// Quy trình
    /// </summary>
    public OpsProcedure Procedure { get; set; } = null!;

    /// <summary>
    /// Phiên bản tài liệu (VD: "1.0")
    /// </summary>
    public string? DocVersion { get; set; }

    /// <summary>
    /// Tên file gốc
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn lưu trữ file
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Kích thước file (bytes)
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Content type (MIME type)
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Thời điểm upload
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
