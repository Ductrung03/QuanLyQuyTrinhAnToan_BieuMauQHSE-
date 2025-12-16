using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Interface cho Audit Log Service
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Lấy danh sách audit log với filter và phân trang
    /// </summary>
    Task<AuditLogPagedResult> GetLogsAsync(AuditLogFilterDto filter);

    /// <summary>
    /// Ghi log hành động
    /// </summary>
    Task LogAsync(
        int? userId,
        string? userName,
        string action,
        string? targetType = null,
        int? targetId = null,
        string? targetName = null,
        string? detail = null,
        string? oldData = null,
        string? newData = null,
        string? ipAddress = null,
        string? userAgent = null);

    /// <summary>
    /// Lấy danh sách các loại action
    /// </summary>
    Task<List<string>> GetActionTypesAsync();

    /// <summary>
    /// Lấy danh sách các loại target
    /// </summary>
    Task<List<string>> GetTargetTypesAsync();
}
