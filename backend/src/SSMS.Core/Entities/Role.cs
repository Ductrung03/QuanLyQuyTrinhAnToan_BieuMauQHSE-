namespace SSMS.Core.Entities;

/// <summary>
/// Vai trò người dùng (Role)
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Tên vai trò (hiển thị)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã vai trò (code để check logic)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả vai trò
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Vai trò hệ thống (không được xóa)
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Navigation: Quyền của role
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Navigation: Users có role này
    /// </summary>
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}
