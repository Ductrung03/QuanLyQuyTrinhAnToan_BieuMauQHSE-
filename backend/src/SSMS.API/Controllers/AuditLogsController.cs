using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller quản lý Nhật ký hệ thống
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(
        IAuditLogService auditLogService,
        ILogger<AuditLogsController> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách nhật ký với filter và phân trang
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] AuditLogFilterDto filter)
    {
        try
        {
            var result = await _auditLogService.GetLogsAsync(filter);
            return Ok(new
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit logs");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách nhật ký"
            });
        }
    }

    /// <summary>
    /// Lấy danh sách các loại hành động
    /// </summary>
    [HttpGet("action-types")]
    public async Task<IActionResult> GetActionTypes()
    {
        try
        {
            var types = await _auditLogService.GetActionTypesAsync();
            return Ok(new
            {
                Success = true,
                Data = types
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting action types");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách loại hành động"
            });
        }
    }

    /// <summary>
    /// Lấy danh sách các loại đối tượng
    /// </summary>
    [HttpGet("target-types")]
    public async Task<IActionResult> GetTargetTypes()
    {
        try
        {
            var types = await _auditLogService.GetTargetTypesAsync();
            return Ok(new
            {
                Success = true,
                Data = types
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting target types");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách loại đối tượng"
            });
        }
    }
}
