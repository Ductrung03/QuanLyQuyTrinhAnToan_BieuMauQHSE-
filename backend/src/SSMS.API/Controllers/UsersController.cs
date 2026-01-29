using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.API.Helpers;
using SSMS.Application.Common;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller for User management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IAuditLogService auditLogService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of users with filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryParams queryParams)
    {
        try
        {
            var result = await _userService.GetPagedAsync(queryParams);
            var response = ApiResponse<PagedResult<UserListDto>>.SuccessResponse(
                result,
                PaginationMeta.Create(queryParams.Page, queryParams.PageSize, result.TotalCount),
                "Lấy danh sách người dùng thành công"
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users list");
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy danh sách người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get all users (for dropdowns)
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            var response = ApiResponse<IEnumerable<UserListDto>>.SuccessResponse(users);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy danh sách người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                var notFoundResponse = ApiResponse<object>.ErrorResponse($"Không tìm thấy người dùng với ID {id}");
                return NotFound(notFoundResponse);
            }

            var response = ApiResponse<UserDto>.SuccessResponse(user);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy thông tin người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get users by Unit ID
    /// </summary>
    [HttpGet("unit/{unitId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUnitId(int unitId)
    {
        try
        {
            var users = await _userService.GetByUnitIdAsync(unitId);
            var response = ApiResponse<IEnumerable<UserListDto>>.SuccessResponse(users);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for unit {UnitId}", unitId);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy danh sách người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        try
        {
            var user = await _userService.CreateAsync(dto);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Create",
                targetType: "User",
                targetId: user.Id,
                targetName: user.Username,
                newData: dto);
            var response = ApiResponse<UserDto>.SuccessResponse(user, "Tạo người dùng thành công");
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            var badRequestResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return BadRequest(badRequestResponse);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return NotFound(notFoundResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi tạo người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Update existing user
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, dto);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Update",
                targetType: "User",
                targetId: id,
                targetName: user.Username,
                newData: dto);
            var response = ApiResponse<UserDto>.SuccessResponse(user, "Cập nhật người dùng thành công");
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return NotFound(notFoundResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi cập nhật người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Deactivate user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id)
    {
        try
        {
            var result = await _userService.DeactivateAsync(id);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Deactivate",
                targetType: "User",
                targetId: id);
            var response = ApiResponse<bool>.SuccessResponse(result, "Vô hiệu hóa người dùng thành công");
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return NotFound(notFoundResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi vô hiệu hóa người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Reactivate user
    /// </summary>
    [HttpPost("{id}/reactivate")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reactivate(int id)
    {
        try
        {
            var result = await _userService.ReactivateAsync(id);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Reactivate",
                targetType: "User",
                targetId: id);
            var response = ApiResponse<bool>.SuccessResponse(result, "Kích hoạt lại người dùng thành công");
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return NotFound(notFoundResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating user {UserId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi kích hoạt lại người dùng", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }
}
