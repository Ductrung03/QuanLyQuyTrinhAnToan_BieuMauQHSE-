using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Service interface for Role management
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Get role by ID
    /// </summary>
    Task<RoleDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get all roles
    /// </summary>
    Task<IEnumerable<RoleDto>> GetAllAsync();

    /// <summary>
    /// Get role by code
    /// </summary>
    Task<RoleDto?> GetByCodeAsync(string code);

    /// <summary>
    /// Create new role
    /// </summary>
    Task<RoleDto> CreateAsync(RoleCreateDto dto);

    /// <summary>
    /// Update existing role
    /// </summary>
    Task<RoleDto> UpdateAsync(int id, RoleUpdateDto dto);

    /// <summary>
    /// Delete role (only non-system roles)
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Assign permissions to role
    /// </summary>
    Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds);

    /// <summary>
    /// Get all permissions of a role
    /// </summary>
    Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId);
}
