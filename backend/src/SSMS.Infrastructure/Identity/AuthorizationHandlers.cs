using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SSMS.Infrastructure.Identity;

/// <summary>
/// Authorization requirement dựa trên Unit
/// </summary>
public class UnitRequirement : IAuthorizationRequirement
{
    public bool RequireSameUnit { get; set; }
    public bool AllowAdminOverride { get; set; } = true;

    public UnitRequirement(bool requireSameUnit = true, bool allowAdminOverride = true)
    {
        RequireSameUnit = requireSameUnit;
        AllowAdminOverride = allowAdminOverride;
    }
}

/// <summary>
/// Handler cho Unit-based Authorization
/// </summary>
public class UnitAuthorizationHandler : AuthorizationHandler<UnitRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UnitRequirement requirement)
    {
        var user = context.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        // Admin có thể access tất cả
        if (requirement.AllowAdminOverride)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        // Kiểm tra UnitId nếu cần
        if (requirement.RequireSameUnit)
        {
            var unitId = user.FindFirst("UnitId")?.Value;
            if (!string.IsNullOrEmpty(unitId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        else
        {
            // Không yêu cầu same unit, chỉ cần authenticated
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Role-based requirement
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; set; }

    public RoleRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}

/// <summary>
/// Handler cho Role-based Authorization
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        var user = context.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        if (role != null && requirement.AllowedRoles.Contains(role))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
