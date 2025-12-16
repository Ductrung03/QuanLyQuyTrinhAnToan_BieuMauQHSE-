using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Interface cho Dashboard Service
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Lấy thống kê tổng quan cho dashboard
    /// </summary>
    Task<DashboardStatsDto> GetStatsAsync(int? currentUserId = null);

    /// <summary>
    /// Lấy danh sách hoạt động gần đây
    /// </summary>
    Task<List<RecentActivityDto>> GetRecentActivitiesAsync(int count = 10);
}
