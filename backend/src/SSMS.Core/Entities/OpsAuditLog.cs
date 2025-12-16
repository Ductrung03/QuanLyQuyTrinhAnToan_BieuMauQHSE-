namespace SSMS.Core.Entities;

/// <summary>
/// Nhật ký hệ thống - ghi lại tất cả các thao tác trong hệ thống
/// </summary>
public class OpsAuditLog : BaseEntity
{
    /// <summary>
    /// ID người thực hiện hành động
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Người thực hiện hành động
    /// </summary>
    public AppUser? User { get; set; }

    /// <summary>
    /// Tên người dùng (lưu trữ để tra cứu nhanh)
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Loại hành động (Create, Edit, Delete, Submit, Approve, Reject, Recall, Login, Logout, ...)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Loại đối tượng tác động (Procedure, Template, Submission, Approval, User, Unit, ...)
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// ID đối tượng bị tác động
    /// </summary>
    public int? TargetId { get; set; }

    /// <summary>
    /// Mô tả ngắn gọn về đối tượng (VD: mã quy trình, tên biểu mẫu, ...)
    /// </summary>
    public string? TargetName { get; set; }

    /// <summary>
    /// Chi tiết hành động
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// Dữ liệu cũ (JSON) - cho các thay đổi
    /// </summary>
    public string? OldData { get; set; }

    /// <summary>
    /// Dữ liệu mới (JSON) - cho các thay đổi
    /// </summary>
    public string? NewData { get; set; }

    /// <summary>
    /// Địa chỉ IP của client
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent của client
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Thời gian thực hiện hành động
    /// </summary>
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;
}
