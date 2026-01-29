using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Application.Common;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller for Permission management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all permissions (flat list)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var permissions = await _permissionService.GetAllAsync();
            return Ok(new { success = true, data = permissions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all permissions");
            return StatusCode(500, new { success = false, error = "Loi khi lay danh sach quyen" });
        }
    }

    /// <summary>
    /// Get permissions grouped by module
    /// </summary>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionGroupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroupedByModule()
    {
        try
        {
            var groupedPermissions = await _permissionService.GetGroupedByModuleAsync();
            return Ok(new { success = true, data = groupedPermissions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grouped permissions");
            return StatusCode(500, new { success = false, error = "Loi khi lay danh sach quyen theo module" });
        }
    }

    /// <summary>
    /// Check if current user has specific permission
    /// </summary>
    [HttpGet("check/{permissionCode}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckPermission(string permissionCode)
    {
        try
        {
            // INPUT VALIDATION: Validate permission code format
            if (string.IsNullOrWhiteSpace(permissionCode) || permissionCode.Length > 100)
            {
                return BadRequest(new { success = false, error = "Ma quyen khong hop le" });
            }

            // Additional validation: Permission codes should be uppercase with dots/underscores
            if (!System.Text.RegularExpressions.Regex.IsMatch(permissionCode, @"^[a-z]+\.[a-z]+$"))
            {
                return BadRequest(new { success = false, error = "Dinh dang ma quyen khong hop le (vd: proc.create)" });
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, error = "Khong xac dinh duoc nguoi dung" });
            }

            var hasPermission = await _permissionService.HasPermissionAsync(userId, permissionCode);
            return Ok(new { success = true, data = hasPermission });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {PermissionCode}", permissionCode);
            return StatusCode(500, new { success = false, error = "Loi khi kiem tra quyen" });
        }
    }
}
