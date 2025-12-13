using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalsController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    /// <summary>
    /// Lấy danh sách task cần phê duyệt của User hiện tại
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetPendingApprovals()
    {
        var userId = GetCurrentUserId();
        var pending = await _approvalService.GetPendingApprovalsAsync(userId);
        return Ok(pending);
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
            return Ok(new { message = "Đã phê duyệt biểu mẫu thành công" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
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
            // Reject requires a reason? User Story doesn't strictly say, but good practice.
            // If explicit validation needed, check !string.IsNullOrEmpty(dto.Note)
            
            await _approvalService.RejectAsync(id, userId, dto.Note);
            return Ok(new { message = "Đã từ chối biểu mẫu" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
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
