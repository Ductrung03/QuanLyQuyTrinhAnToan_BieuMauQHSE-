namespace SSMS.Application.DTOs;

/// <summary>
/// DTO for Permission (Get)
/// </summary>
public record PermissionDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Module { get; init; } = string.Empty;
    public string? Description { get; init; }
}

/// <summary>
/// DTO for User Permission Override
/// </summary>
public record UserPermissionDto
{
    public int UserId { get; init; }
    public int PermissionId { get; init; }
    public string PermissionCode { get; init; } = string.Empty;
    public string PermissionName { get; init; } = string.Empty;
    public bool IsGranted { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO for assigning permissions to role/user
/// </summary>
public record AssignPermissionsDto
{
    public IEnumerable<int> PermissionIds { get; init; } = Array.Empty<int>();
}

/// <summary>
/// DTO for grouped permissions by module
/// </summary>
public record PermissionGroupDto
{
    public string Module { get; init; } = string.Empty;
    public IEnumerable<PermissionDto> Permissions { get; init; } = Array.Empty<PermissionDto>();
}
