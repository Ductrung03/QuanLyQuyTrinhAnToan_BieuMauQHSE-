namespace SSMS.Application.DTOs;

/// <summary>
/// DTO cho Dashboard statistics
/// </summary>
public class DashboardStatsDto
{
    public int TotalProcedures { get; set; }
    public int TotalTemplates { get; set; }
    public int TotalSubmissions { get; set; }
    public int PendingApprovals { get; set; }
    public int ApprovedSubmissions { get; set; }
    public int RejectedSubmissions { get; set; }
    public int TotalUsers { get; set; }
    public int TotalUnits { get; set; }
    
    // Statistics by state
    public int DraftProcedures { get; set; }
    public int SubmittedProcedures { get; set; }
    public int ApprovedProcedures { get; set; }
    
    // Recent activities
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

/// <summary>
/// DTO cho Recent Activity
/// </summary>
public class RecentActivityDto
{
    public DateTime Time { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Target { get; set; }
    public string? Detail { get; set; }
}
