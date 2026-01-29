namespace SSMS.Application.DTOs;

/// <summary>
/// DTO for Role (Get)
/// </summary>
public record RoleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsSystemRole { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for creating Role
/// </summary>
public record RoleCreateDto
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
}

/// <summary>
/// DTO for updating Role
/// </summary>
public record RoleUpdateDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
