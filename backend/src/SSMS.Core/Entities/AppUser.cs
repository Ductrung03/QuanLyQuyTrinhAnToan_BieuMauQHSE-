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
    public string? Email { get; set; }

    /// <summary>
    /// Họ và tên đầy đủ
    /// </summary>
    public string? FullName { get; set; }

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
    public int? UnitId { get; set; }

    /// <summary>
    /// Đơn vị mà user thuộc về
    /// </summary>
    public Unit? Unit { get; set; }

    /// <summary>
    /// Vai trò của user (Admin, Manager, User) - DEPRECATED: Sử dụng RoleId thay thế
    /// </summary>
    [Obsolete("Use RoleId instead")]
    public string? Role { get; set; }

    /// <summary>
    /// ID vai trò (FK to Role table)
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// Navigation: Role của user
    /// </summary>
    public Role? RoleEntity { get; set; }

    /// <summary>
    /// Navigation: User permission overrides
    /// </summary>
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lần đăng nhập cuối
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
