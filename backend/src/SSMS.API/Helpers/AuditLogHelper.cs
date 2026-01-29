using System.Security.Claims;
using System.Text.Json;
using SSMS.Application.Services;

namespace SSMS.API.Helpers;

public static class AuditLogHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static Task LogAsync(
        IAuditLogService auditLogService,
        HttpContext? context,
        string action,
        string? targetType = null,
        int? targetId = null,
        string? targetName = null,
        string? detail = null,
        object? oldData = null,
        object? newData = null,
        int? userIdOverride = null,
        string? userNameOverride = null)
    {
        if (context == null)
        {
            return Task.CompletedTask;
        }

        var userId = userIdOverride ?? TryGetUserId(context.User);
        var userName = userNameOverride
            ?? context.User.FindFirstValue(ClaimTypes.GivenName)
            ?? context.User.FindFirstValue(ClaimTypes.Name)
            ?? context.User.Identity?.Name;

        var oldJson = oldData == null ? null : JsonSerializer.Serialize(oldData, JsonOptions);
        var newJson = newData == null ? null : JsonSerializer.Serialize(newData, JsonOptions);

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();

        return auditLogService.LogAsync(
            userId,
            userName,
            action,
            targetType,
            targetId,
            targetName,
            detail,
            oldJson,
            newJson,
            ipAddress,
            userAgent);
    }

    private static int? TryGetUserId(ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) ? id : null;
    }
}
