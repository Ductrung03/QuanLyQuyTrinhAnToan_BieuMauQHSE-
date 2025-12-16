using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSMS.Application.DTOs;
using SSMS.Application.Services;
using SSMS.Core.Entities;
using SSMS.Infrastructure.Data;

namespace SSMS.Infrastructure.Services;

/// <summary>
/// Service xử lý Audit Log
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(AppDbContext context, ILogger<AuditLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách audit log với filter và phân trang
    /// </summary>
    public async Task<AuditLogPagedResult> GetLogsAsync(AuditLogFilterDto filter)
    {
        var query = _context.OpsAuditLogs
            .Include(l => l.User)
            .AsQueryable();

        // Apply filters
        if (filter.UserId.HasValue)
        {
            query = query.Where(l => l.UserId == filter.UserId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Action))
        {
            query = query.Where(l => l.Action == filter.Action);
        }

        if (!string.IsNullOrEmpty(filter.TargetType))
        {
            query = query.Where(l => l.TargetType == filter.TargetType);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(l => l.ActionTime >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(l => l.ActionTime <= filter.ToDate.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination and ordering
        var items = await query
            .OrderByDescending(l => l.ActionTime)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new AuditLogDto
            {
                Id = l.Id,
                UserId = l.UserId,
                UserName = l.UserName ?? (l.User != null ? l.User.FullName ?? l.User.Username : "Unknown"),
                Action = l.Action,
                TargetType = l.TargetType,
                TargetId = l.TargetId,
                TargetName = l.TargetName,
                Detail = l.Detail,
                IpAddress = l.IpAddress,
                ActionTime = l.ActionTime
            })
            .ToListAsync();

        return new AuditLogPagedResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    /// <summary>
    /// Ghi log hành động
    /// </summary>
    public async Task LogAsync(
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
        string? userAgent = null)
    {
        try
        {
            var log = new OpsAuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                TargetName = targetName,
                Detail = detail,
                OldData = oldData,
                NewData = newData,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ActionTime = DateTime.UtcNow
            };

            _context.OpsAuditLogs.Add(log);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Audit Log: {Action} by {UserName} on {TargetType}/{TargetId} ({TargetName})",
                action, userName, targetType, targetId, targetName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log");
            // Don't throw - logging should not break the main flow
        }
    }

    /// <summary>
    /// Lấy danh sách các loại action
    /// </summary>
    public async Task<List<string>> GetActionTypesAsync()
    {
        return await _context.OpsAuditLogs
            .Select(l => l.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách các loại target
    /// </summary>
    public async Task<List<string>> GetTargetTypesAsync()
    {
        return await _context.OpsAuditLogs
            .Where(l => l.TargetType != null)
            .Select(l => l.TargetType!)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }
}
