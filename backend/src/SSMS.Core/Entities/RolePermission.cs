namespace SSMS.Core.Entities;

/// <summary>
/// Bảng trung gian: Role-Permission mapping (many-to-many)
/// </summary>
public class RolePermission
{
    /// <summary>
    /// ID vai trò
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// ID quyền
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Navigation: Role
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Navigation: Permission
    /// </summary>
    public Permission Permission { get; set; } = null!;

    /// <summary>
    /// Ngày gán quyền
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
