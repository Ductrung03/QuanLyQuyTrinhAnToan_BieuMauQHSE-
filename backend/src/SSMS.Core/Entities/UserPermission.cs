namespace SSMS.Core.Entities;

/// <summary>
/// Ghi đè quyền của user (User Permission Override)
/// Cho phép grant hoặc revoke quyền cụ thể cho user (override role)
/// </summary>
public class UserPermission : BaseEntity
{
    /// <summary>
    /// ID người dùng
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID quyền
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Grant (true) hoặc Revoke (false)
    /// </summary>
    public bool IsGranted { get; set; }

    /// <summary>
    /// Navigation: User
    /// </summary>
    public AppUser User { get; set; } = null!;

    /// <summary>
    /// Navigation: Permission
    /// </summary>
    public Permission Permission { get; set; } = null!;
}
