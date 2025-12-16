using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSMS.Application.DTOs;
using SSMS.Application.Services;
using SSMS.Infrastructure.Data;

namespace SSMS.Infrastructure.Services;

/// <summary>
/// Service xử lý Dashboard statistics
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(AppDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy thống kê tổng quan cho dashboard
    /// </summary>
    public async Task<DashboardStatsDto> GetStatsAsync(int? currentUserId = null)
    {
        var stats = new DashboardStatsDto
        {
            // Procedure statistics
            TotalProcedures = await _context.OpsProcedures.CountAsync(),
            DraftProcedures = await _context.OpsProcedures.CountAsync(p => p.State == "Draft"),
            SubmittedProcedures = await _context.OpsProcedures.CountAsync(p => p.State == "Submitted"),
            ApprovedProcedures = await _context.OpsProcedures.CountAsync(p => p.State == "Approved"),

            // Template statistics
            TotalTemplates = await _context.OpsTemplates.CountAsync(),

            // Submission statistics
            TotalSubmissions = await _context.OpsSubmissions.CountAsync(),
            PendingApprovals = await _context.OpsSubmissions.CountAsync(s => s.Status == "Submitted"),
            ApprovedSubmissions = await _context.OpsSubmissions.CountAsync(s => s.Status == "Approved"),
            RejectedSubmissions = await _context.OpsSubmissions.CountAsync(s => s.Status == "Rejected"),

            // User and Unit statistics
            TotalUsers = await _context.AppUsers.CountAsync(),
            TotalUnits = await _context.Units.CountAsync()
        };

        // Get recent activities with error handling
        try
        {
            stats.RecentActivities = await GetRecentActivitiesAsync(10);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load recent activities, returning empty list");
            stats.RecentActivities = new List<RecentActivityDto>();
        }

        return stats;
    }

    /// <summary>
    /// Lấy danh sách hoạt động gần đây
    /// </summary>
    public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync(int count = 10)
    {
        // Try to get from audit logs first, but handle case where table doesn't exist
        try
        {
            var hasAuditLogs = await _context.OpsAuditLogs.AnyAsync();
            
            if (hasAuditLogs)
            {
                return await _context.OpsAuditLogs
                    .OrderByDescending(l => l.ActionTime)
                    .Take(count)
                    .Select(l => new RecentActivityDto
                    {
                        Time = l.ActionTime,
                        UserName = l.UserName,
                        Action = l.Action,
                        Target = l.TargetName,
                        Detail = l.Detail
                    })
                    .ToListAsync();
            }
        }
        catch (Exception ex)
        {
            // OpsAuditLog table might not exist yet - fall through to fallback
            _logger.LogDebug(ex, "OpsAuditLog table not available, using fallback");
        }

        // Fallback: Generate recent activities from submissions and approvals
        try
        {
            var recentSubmissions = await _context.OpsSubmissions
                .Include(s => s.SubmittedByUser)
                .Include(s => s.Procedure)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(count / 2)
                .Select(s => new RecentActivityDto
                {
                    Time = s.SubmittedAt,
                    UserName = s.SubmittedByUser != null ? s.SubmittedByUser.FullName ?? s.SubmittedByUser.Username : "Unknown",
                    Action = "Submit",
                    Target = s.Title,
                    Detail = s.Procedure != null ? s.Procedure.Code : null
                })
                .ToListAsync();

            var recentApprovals = await _context.OpsApprovals
                .Include(a => a.Approver)
                .Include(a => a.Submission)
                .OrderByDescending(a => a.ActionDate)
                .Take(count / 2)
                .Select(a => new RecentActivityDto
                {
                    Time = a.ActionDate,
                    UserName = a.Approver != null ? a.Approver.FullName ?? a.Approver.Username : "Unknown",
                    Action = a.Action,
                    Target = a.Submission != null ? a.Submission.Title : "Unknown",
                    Detail = a.Note
                })
                .ToListAsync();

            return recentSubmissions
                .Concat(recentApprovals)
                .OrderByDescending(a => a.Time)
                .Take(count)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load recent activities from submissions/approvals");
            return new List<RecentActivityDto>();
        }
    }
}
