using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Service interface for Unit management
/// </summary>
public interface IUnitService
{
    /// <summary>
    /// Get all units (for dropdowns and lists)
    /// </summary>
    Task<IEnumerable<UnitListDto>> GetAllAsync();

    /// <summary>
    /// Get unit by ID with details
    /// </summary>
    Task<UnitDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get units by type (Ship, Department, etc.)
    /// </summary>
    Task<IEnumerable<UnitListDto>> GetByTypeAsync(string unitType);

    /// <summary>
    /// Get child units of a parent unit
    /// </summary>
    Task<IEnumerable<UnitListDto>> GetChildUnitsAsync(int parentUnitId);

    /// <summary>
    /// Get unit hierarchy (tree structure)
    /// </summary>
    Task<IEnumerable<UnitDto>> GetHierarchyAsync();

    /// <summary>
    /// Check if unit code already exists
    /// </summary>
    Task<bool> UnitCodeExistsAsync(string unitCode, int? excludeUnitId = null);

    /// <summary>
    /// Create new unit
    /// </summary>
    Task<UnitDto> CreateAsync(UnitCreateDto dto);

    /// <summary>
    /// Update existing unit
    /// </summary>
    Task<UnitDto> UpdateAsync(int id, UnitUpdateDto dto);

    /// <summary>
    /// Delete unit (with validation)
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
