namespace SSMS.Core.Entities;

/// <summary>
/// Quy trình vận hành (OPS-01, OPS-02, ...)
/// </summary>
public class OpsProcedure : BaseEntity
{
    /// <summary>
    /// Mã quy trình (VD: "OPS-01", "OPS-02")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Tên quy trình
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ID người chủ trì
    /// </summary>
    public int? OwnerUserId { get; set; }

    /// <summary>
    /// Người chủ trì
    /// </summary>
    public AppUser? OwnerUser { get; set; }

    /// <summary>
    /// Phiên bản (VD: "1.0", "0.8")
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Trạng thái (Draft, Submitted, Approved, Rejected)
    /// </summary>
    public string State { get; set; } = "Draft";

    /// <summary>
    /// ID người lập
    /// </summary>
    public int? AuthorUserId { get; set; }

    /// <summary>
    /// Người lập
    /// </summary>
    public AppUser? AuthorUser { get; set; }

    /// <summary>
    /// ID người duyệt
    /// </summary>
    public int? ApproverUserId { get; set; }

    /// <summary>
    /// Người duyệt
    /// </summary>
    public AppUser? ApproverUser { get; set; }

    /// <summary>
    /// Ngày tạo quy trình
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Ngày phát hành
    /// </summary>
    public DateTime? ReleasedDate { get; set; }

    /// <summary>
    /// Mô tả (song ngữ, mục đích, phạm vi)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tài liệu đính kèm
    /// </summary>
    public ICollection<OpsProcedureDocument> Documents { get; set; } = new List<OpsProcedureDocument>();

    /// <summary>
    /// Biểu mẫu/Checklist gắn với quy trình
    /// </summary>
    public ICollection<OpsTemplate> Templates { get; set; } = new List<OpsTemplate>();
}
