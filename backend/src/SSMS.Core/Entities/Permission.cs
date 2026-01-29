namespace SSMS.Core.Entities;

/// <summary>
/// Quyền hạn (Permission)
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Tên quyền (hiển thị)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã quyền (code để check logic, ví dụ: proc.create)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Module/nhóm quyền (Procedures, Templates, Submissions, etc.)
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả quyền
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Navigation: Roles có quyền này
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Navigation: User permission overrides
    /// </summary>
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
