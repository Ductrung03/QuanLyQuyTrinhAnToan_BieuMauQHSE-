using SSMS.Application.Common;
using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Service interface for User management
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get paginated list of users with filtering
    /// </summary>
    Task<PagedResult<UserListDto>> GetPagedAsync(UserQueryParams queryParams);

    /// <summary>
    /// Get all users (for dropdowns)
    /// </summary>
    Task<IEnumerable<UserListDto>> GetAllAsync();

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get users by Unit ID
    /// </summary>
    Task<IEnumerable<UserListDto>> GetByUnitIdAsync(int unitId);

    /// <summary>
    /// Check if username already exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);

    /// <summary>
    /// Create new user
    /// </summary>
    Task<UserDto> CreateAsync(UserCreateDto dto);

    /// <summary>
    /// Update existing user
    /// </summary>
    Task<UserDto> UpdateAsync(int id, UserUpdateDto dto);

    /// <summary>
    /// Deactivate user (soft delete)
    /// </summary>
    Task<bool> DeactivateAsync(int id);

    /// <summary>
    /// Reactivate user
    /// </summary>
    Task<bool> ReactivateAsync(int id);
}
