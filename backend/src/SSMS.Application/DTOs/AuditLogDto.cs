namespace SSMS.Application.DTOs;

/// <summary>
/// DTO cho Audit Log (read)
/// </summary>
public class AuditLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? TargetType { get; set; }
    public int? TargetId { get; set; }
    public string? TargetName { get; set; }
    public string? Detail { get; set; }
    public string? IpAddress { get; set; }
    public DateTime ActionTime { get; set; }
}

/// <summary>
/// DTO cho việc filter audit log
/// </summary>
public class AuditLogFilterDto
{
    public int? UserId { get; set; }
    public string? Action { get; set; }
    public string? TargetType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

/// <summary>
/// DTO response với phân trang
/// </summary>
public class AuditLogPagedResult
{
    public List<AuditLogDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
