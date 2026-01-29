using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.API.Helpers;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller cho quản lý biểu mẫu
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(
        ISubmissionService submissionService,
        IAuditLogService auditLogService,
        ILogger<SubmissionsController> logger)
    {
        _submissionService = submissionService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách biểu mẫu của người dùng hiện tại
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMySubmissions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var submissions = await _submissionService.GetMySubmissionsAsync(userId);
            
            return Ok(new
            {
                Success = true,
                Data = submissions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user submissions");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Lấy chi tiết biểu mẫu theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var submission = await _submissionService.GetByIdAsync(id);
            
            if (submission == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Không tìm thấy biểu mẫu"
                });
            }

            return Ok(new
            {
                Success = true,
                Data = submission
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submission {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy chi tiết biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Tạo mới biểu mẫu
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] SubmissionCreateDto dto,
        [FromForm] List<IFormFile>? files)
    {
        try
        {
            var userId = GetCurrentUserId();
            var submission = await _submissionService.CreateAsync(dto, userId, files);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Submit",
                targetType: "Submission",
                targetId: submission.Id,
                targetName: submission.Title,
                newData: dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = submission.Id },
                new
                {
                    Success = true,
                    Data = submission,
                    Message = "Nộp biểu mẫu thành công"
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi nộp biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Thu hồi biểu mẫu
    /// </summary>
    [HttpPost("{id}/recall")]
    public async Task<IActionResult> Recall(int id, [FromBody] RecallDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _submissionService.RecallAsync(id, userId, dto.Reason);

            await AuditLogHelper.LogAsync(
                _auditLogService,
                HttpContext,
                action: "Recall",
                targetType: "Submission",
                targetId: id,
                detail: dto.Reason);

            return Ok(new
            {
                Success = true,
                Message = "Thu hồi biểu mẫu thành công"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalling submission {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi thu hồi biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Kiểm tra có thể thu hồi biểu mẫu không
    /// </summary>
    [HttpGet("{id}/can-recall")]
    public async Task<IActionResult> CanRecall(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var canRecall = await _submissionService.CanRecallAsync(id, userId);

            return Ok(new
            {
                Success = true,
                Data = canRecall
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking recall status for submission {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi kiểm tra trạng thái thu hồi"
            });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            // Fallback for development/testing if claims are missing, or strict throw
            // Given MockAuthService sets NameIdentifier, we should expect it.
             throw new UnauthorizedAccessException("Không xác định được User ID");
            
        return int.Parse(userIdClaim.Value);
    }
}
