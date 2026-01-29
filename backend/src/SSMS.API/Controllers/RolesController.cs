using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.API.Helpers;
using SSMS.Application.Common;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller for Role management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, IAuditLogService auditLogService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(new { success = true, data = roles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return StatusCode(500, new { success = false, error = "Loi khi lay danh sach vai tro" });
        }
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { success = false, error = $"Khong tim thay vai tro voi ID {id}" });

            return Ok(new { success = true, data = role });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role {RoleId}", id);
            return StatusCode(500, new { success = false, error = "Loi khi lay thong tin vai tro" });
        }
    }

    /// <summary>
    /// Get role by code
    /// </summary>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            // INPUT VALIDATION: Validate role code format
            if (string.IsNullOrWhiteSpace(code) || code.Length > 50)
            {
                return BadRequest(new { success = false, error = "Ma vai tro khong hop le" });
            }

            // Role codes must be uppercase, start with letter, contain only letters/numbers/underscores
            if (!System.Text.RegularExpressions.Regex.IsMatch(code, @"^[A-Z][A-Z0-9_]*$"))
            {
                return BadRequest(new { success = false, error = "Dinh dang ma vai tro khong hop le (VD: ADMIN, SHIP_CAPTAIN)" });
            }

            var role = await _roleService.GetByCodeAsync(code);
            if (role == null)
                return NotFound(new { success = false, error = $"Khong tim thay vai tro voi ma '{code}'" });

            return Ok(new { success = true, data = role });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by code {Code}", code);
            return StatusCode(500, new { success = false, error = "Loi khi lay thong tin vai tro" });
        }
    }

    /// <summary>
    /// Create new role (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
    {
        try
        {
            var role = await _roleService.CreateAsync(dto);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Create",
                targetType: "Role",
                targetId: role.Id,
                targetName: role.Name,
                newData: dto);
            return CreatedAtAction(nameof(GetById), new { id = role.Id },
                new { success = true, data = role });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {@Dto}", dto);
            return StatusCode(500, new { success = false, error = "Loi khi tao vai tro" });
        }
    }

    /// <summary>
    /// Update role (Admin only)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] RoleUpdateDto dto)
    {
        try
        {
            var role = await _roleService.UpdateAsync(id, dto);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Update",
                targetType: "Role",
                targetId: id,
                targetName: role.Name,
                newData: dto);
            return Ok(new { success = true, data = role });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId} {@Dto}", id, dto);
            return StatusCode(500, new { success = false, error = "Loi khi cap nhat vai tro" });
        }
    }

    /// <summary>
    /// Delete role (Admin only)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _roleService.DeleteAsync(id);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Delete",
                targetType: "Role",
                targetId: id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, new { success = false, error = "Loi khi xoa vai tro" });
        }
    }

    /// <summary>
    /// Assign permissions to role (Admin only)
    /// </summary>
    [HttpPost("{id:int}/permissions")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] AssignPermissionsDto dto)
    {
        try
        {
            await _roleService.AssignPermissionsAsync(id, dto.PermissionIds);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "AssignPermissions",
                targetType: "Role",
                targetId: id,
                detail: $"Permissions: {string.Join(',', dto.PermissionIds)}");
            return Ok(new { success = true, message = "Gan quyen thanh cong" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role {RoleId}", id);
            return StatusCode(500, new { success = false, error = "Loi khi gan quyen cho vai tro" });
        }
    }

    /// <summary>
    /// Get permissions of a role
    /// </summary>
    [HttpGet("{id:int}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRolePermissions(int id)
    {
        try
        {
            var permissions = await _roleService.GetRolePermissionsAsync(id);
            return Ok(new { success = true, data = permissions });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for role {RoleId}", id);
            return StatusCode(500, new { success = false, error = "Loi khi lay danh sach quyen" });
        }
    }
}
