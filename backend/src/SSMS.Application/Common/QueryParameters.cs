namespace SSMS.Application.Common;

/// <summary>
/// Base pagination parameters for list queries
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    /// <summary>
    /// Page number (1-based, default: 1)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page (default: 20, max: 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }

    /// <summary>
    /// Calculate skip count for database query
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Take count for database query
    /// </summary>
    public int Take => PageSize;
}

/// <summary>
/// Query parameters for Procedure list
/// </summary>
public class ProcedureQueryParams : PaginationParams
{
    /// <summary>
    /// Search by Code or Name
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by State (Draft, Submitted, Approved, Rejected, Archived)
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Filter by Owner User ID
    /// </summary>
    public int? OwnerUserId { get; set; }

    /// <summary>
    /// Filter by Approver User ID
    /// </summary>
    public int? ApproverUserId { get; set; }

    /// <summary>
    /// Sort by field (Code, Name, CreatedAt, ReleasedDate)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort descending (default: true - newest first)
    /// </summary>
    public bool SortDesc { get; set; } = true;
}

/// <summary>
/// Query parameters for Template list
/// </summary>
public class TemplateQueryParams : PaginationParams
{
    /// <summary>
    /// Search by TemplateNo or Name
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by Procedure ID
    /// </summary>
    public int? ProcedureId { get; set; }

    /// <summary>
    /// Filter by Template Type (Form, Checklist, Report, Other)
    /// </summary>
    public string? TemplateType { get; set; }

    /// <summary>
    /// Filter by State
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort descending
    /// </summary>
    public bool SortDesc { get; set; } = true;
}

/// <summary>
/// Query parameters for Submission list
/// </summary>
public class SubmissionQueryParams : PaginationParams
{
    /// <summary>
    /// Search by Title
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by Status (Submitted, Approved, Rejected, Recalled)
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by Procedure ID
    /// </summary>
    public int? ProcedureId { get; set; }

    /// <summary>
    /// Filter by Template ID
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Filter by Submitted User ID
    /// </summary>
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// Filter from date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter to date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Filter by current user's unit (for visibility)
    /// </summary>
    public int? CurrentUnitId { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort descending
    /// </summary>
    public bool SortDesc { get; set; } = true;
}

/// <summary>
/// Query parameters for User list
/// </summary>
public class UserQueryParams : PaginationParams
{
    /// <summary>
    /// Search by Username, FullName, Email
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by Unit ID
    /// </summary>
    public int? UnitId { get; set; }

    /// <summary>
    /// Filter by Role
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Filter by IsActive status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort descending
    /// </summary>
    public bool SortDesc { get; set; } = false;
}

/// <summary>
/// Query parameters for Audit Log list
/// </summary>
public class AuditLogQueryParams : PaginationParams
{
    /// <summary>
    /// Filter by User ID
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Filter by Action type
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Filter by Target Type
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// Filter by Target ID
    /// </summary>
    public int? TargetId { get; set; }

    /// <summary>
    /// Filter from date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter to date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Sort descending (default: true - newest first)
    /// </summary>
    public bool SortDesc { get; set; } = true;
}
