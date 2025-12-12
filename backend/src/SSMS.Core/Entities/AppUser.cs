namespace SSMS.Core.Entities;

/// <summary>
/// Người dùng hệ thống
/// </summary>
public class AppUser : BaseEntity
{
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Họ và tên đầy đủ
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Mật khẩu đã hash (cho tương lai, hiện tại dùng Mock Auth)
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Chức vụ
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// ID đơn vị mà user thuộc về
    /// </summary>
    public int UnitId { get; set; }

    /// <summary>
    /// Đơn vị mà user thuộc về
    /// </summary>
    public Unit Unit { get; set; } = null!;

    /// <summary>
    /// Vai trò của user (Admin, Manager, User)
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lần đăng nhập cuối
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
