using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Application.Services;
using System.Security.Claims;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller cho Dashboard - thống kê tổng quan
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy thống kê tổng quan cho dashboard
    /// </summary>
    [HttpGet("stats")]
    [AllowAnonymous] // Allow anonymous for development - remove in production
    public async Task<IActionResult> GetStats()
    {
        try
        {
            // Lấy user ID từ claims nếu cần
            int? currentUserId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid))
            {
                currentUserId = uid;
            }

            var stats = await _dashboardService.GetStatsAsync(currentUserId);
            return Ok(new
            {
                Success = true,
                Data = stats
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy thống kê dashboard"
            });
        }
    }

    /// <summary>
    /// Lấy danh sách hoạt động gần đây
    /// </summary>
    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 10)
    {
        try
        {
            var activities = await _dashboardService.GetRecentActivitiesAsync(count);
            return Ok(new
            {
                Success = true,
                Data = activities
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent activities");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy hoạt động gần đây"
            });
        }
    }
}
