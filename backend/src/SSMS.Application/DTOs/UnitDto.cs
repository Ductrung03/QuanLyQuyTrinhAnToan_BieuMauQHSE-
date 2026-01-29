namespace SSMS.Application.DTOs;

/// <summary>
/// DTO for Unit (full details)
/// </summary>
public class UnitDto
{
    public int Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty; // Ship, Department, etc.
    public string? Description { get; set; }
    public int? ParentUnitId { get; set; }
    public string? ParentUnitName { get; set; }
    public bool IsActive { get; set; }
    public List<UnitDto> ChildUnits { get; set; } = new();
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Unit
/// </summary>
public class UnitCreateDto
{
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentUnitId { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating Unit
/// </summary>
public class UnitUpdateDto
{
    public string UnitName { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentUnitId { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for Unit list (simplified)
/// </summary>
public class UnitListDto
{
    public int Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public string? ParentUnitName { get; set; }
    public int UserCount { get; set; }
    public bool IsActive { get; set; }
}
