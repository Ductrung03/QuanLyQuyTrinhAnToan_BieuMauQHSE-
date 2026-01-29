namespace SSMS.Application.DTOs;

/// <summary>
/// DTO for User (full details)
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? Role { get; set; }
    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new User
/// </summary>
public class UserCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? Role { get; set; }
    public int? UnitId { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating User
/// </summary>
public class UserUpdateDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? Role { get; set; }
    public int? UnitId { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for User list (simplified)
/// </summary>
public class UserListDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
    public string? Role { get; set; }
    public string? UnitName { get; set; }
    public bool IsActive { get; set; }
}
