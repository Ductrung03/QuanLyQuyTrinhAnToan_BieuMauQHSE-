using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Service interface for Permission management
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Get all permissions
    /// </summary>
    Task<IEnumerable<PermissionDto>> GetAllAsync();

    /// <summary>
    /// Get permissions grouped by module
    /// </summary>
    Task<IEnumerable<PermissionGroupDto>> GetGroupedByModuleAsync();

    /// <summary>
    /// Get all effective permission codes for a user (role + overrides)
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionCodesAsync(int userId);

    /// <summary>
    /// Check if user has a specific permission
    /// </summary>
    Task<bool> HasPermissionAsync(int userId, string permissionCode);

    /// <summary>
    /// Grant permission to user (override)
    /// </summary>
    Task GrantPermissionToUserAsync(int userId, int permissionId);

    /// <summary>
    /// Revoke permission from user (override)
    /// </summary>
    Task RevokePermissionFromUserAsync(int userId, int permissionId);

    /// <summary>
    /// Get all permission overrides for a user
    /// </summary>
    Task<IEnumerable<UserPermissionDto>> GetUserPermissionOverridesAsync(int userId);

    /// <summary>
    /// Remove permission override (back to role default)
    /// </summary>
    Task RemoveUserPermissionOverrideAsync(int userId, int permissionId);
}
