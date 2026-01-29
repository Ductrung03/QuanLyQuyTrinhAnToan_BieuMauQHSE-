using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.API.Helpers;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _approvalService;
    private readonly IAuditLogService _auditLogService;

    public ApprovalsController(IApprovalService approvalService, IAuditLogService auditLogService)
    {
        _approvalService = approvalService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Lấy danh sách task cần phê duyệt của User hiện tại
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        try
        {
            var userId = GetCurrentUserId();
            var pending = await _approvalService.GetPendingApprovalsAsync(userId);
            return Ok(new
            {
                Success = true,
                Data = pending
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách phê duyệt: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Phê duyệt biểu mẫu
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApprovalActionDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _approvalService.ApproveAsync(id, userId, dto.Note);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Approve",
                targetType: "Submission",
                targetId: id,
                detail: dto.Note);
            return Ok(new 
            { 
                Success = true,
                Message = "Đã phê duyệt biểu mẫu thành công" 
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Success = false, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "Lỗi server", Error = ex.Message });
        }
    }

    /// <summary>
    /// Từ chối biểu mẫu
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] ApprovalActionDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _approvalService.RejectAsync(id, userId, dto.Note);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Reject",
                targetType: "Submission",
                targetId: id,
                detail: dto.Note);
            return Ok(new 
            { 
                Success = true,
                Message = "Đã từ chối biểu mẫu" 
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Success = false, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "Lỗi server", Error = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("Không xác định được User ID");
            
        return int.Parse(userIdClaim.Value);
    }
}
