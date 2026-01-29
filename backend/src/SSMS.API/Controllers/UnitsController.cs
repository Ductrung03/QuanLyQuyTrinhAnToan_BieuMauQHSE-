using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.API.Helpers;
using SSMS.Application.Common;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller for Unit management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<UnitsController> _logger;

    public UnitsController(
        IUnitService unitService,
        IAuditLogService auditLogService,
        ILogger<UnitsController> logger)
    {
        _unitService = unitService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Get all units (for lists and dropdowns)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var units = await _unitService.GetAllAsync();
            var response = ApiResponse<IEnumerable<UnitListDto>>.SuccessResponse(units, "Lấy danh sách đơn vị thành công");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting units list");
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy danh sách đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var unit = await _unitService.GetByIdAsync(id);
            if (unit == null)
            {
                var notFoundResponse = ApiResponse<object>.ErrorResponse($"Không tìm thấy đơn vị với ID {id}");
                return NotFound(notFoundResponse);
            }

            var response = ApiResponse<UnitDto>.SuccessResponse(unit);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit {UnitId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy thông tin đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get units by type (Ship, Department, etc.)
    /// </summary>
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByType(string type)
    {
        try
        {
            var units = await _unitService.GetByTypeAsync(type);
            var response = ApiResponse<IEnumerable<UnitListDto>>.SuccessResponse(units);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting units by type {UnitType}", type);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy danh sách đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get child units of a parent unit
    /// </summary>
    [HttpGet("{id}/children")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildUnits(int id)
    {
        try
        {
            var units = await _unitService.GetChildUnitsAsync(id);
            var response = ApiResponse<IEnumerable<UnitListDto>>.SuccessResponse(units);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting child units for {ParentId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy danh sách đơn vị con", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get unit hierarchy (tree structure)
    /// </summary>
    [HttpGet("hierarchy")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHierarchy()
    {
        try
        {
            var hierarchy = await _unitService.GetHierarchyAsync();
            var response = ApiResponse<IEnumerable<UnitDto>>.SuccessResponse(hierarchy, "Lấy cây phân cấp đơn vị thành công");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit hierarchy");
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi lấy cây phân cấp đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Create new unit
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UnitCreateDto dto)
    {
        try
        {
            var unit = await _unitService.CreateAsync(dto);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Create",
                targetType: "Unit",
                targetId: unit.Id,
                targetName: unit.UnitName,
                newData: dto);
            var response = ApiResponse<UnitDto>.SuccessResponse(unit, "Tạo đơn vị thành công");
            return CreatedAtAction(nameof(GetById), new { id = unit.Id }, response);
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
            _logger.LogError(ex, "Error creating unit");
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi tạo đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Update existing unit
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UnitUpdateDto dto)
    {
        try
        {
            var unit = await _unitService.UpdateAsync(id, dto);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Update",
                targetType: "Unit",
                targetId: id,
                targetName: unit.UnitName,
                newData: dto);
            var response = ApiResponse<UnitDto>.SuccessResponse(unit, "Cập nhật đơn vị thành công");
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return NotFound(notFoundResponse);
        }
        catch (InvalidOperationException ex)
        {
            var badRequestResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return BadRequest(badRequestResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating unit {UnitId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi cập nhật đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete unit (with validation)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _unitService.DeleteAsync(id);
            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Delete",
                targetType: "Unit",
                targetId: id);
            var response = ApiResponse<bool>.SuccessResponse(result, "Xóa đơn vị thành công");
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return NotFound(notFoundResponse);
        }
        catch (InvalidOperationException ex)
        {
            var badRequestResponse = ApiResponse<object>.ErrorResponse(ex.Message);
            return BadRequest(badRequestResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting unit {UnitId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse("Lỗi khi xóa đơn vị", ex.Message);
            return StatusCode(500, errorResponse);
        }
    }
}
