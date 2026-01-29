using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service implementation for Unit management
/// </summary>
public class UnitService : IUnitService
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UnitListDto>> GetAllAsync()
    {
        var units = await _unitOfWork.Units.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();

        return units.Select(u => new UnitListDto
        {
            Id = u.Id,
            UnitCode = u.Code,
            UnitName = u.Name,
            UnitType = u.Type,
            ParentUnitName = u.ParentUnit?.Name,
            UserCount = users.Count(user => user.UnitId == u.Id),
            IsActive = u.IsActive
        }).OrderBy(u => u.UnitName);
    }

    public async Task<UnitDto?> GetByIdAsync(int id)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(id);
        if (unit == null) return null;

        // Load parent unit if exists
        Unit? parentUnit = null;
        if (unit.ParentUnitId.HasValue)
        {
            parentUnit = await _unitOfWork.Units.GetByIdAsync(unit.ParentUnitId.Value);
        }

        // Load child units
        var allUnits = await _unitOfWork.Units.GetAllAsync();
        var childUnits = allUnits.Where(u => u.ParentUnitId == id).ToList();

        // Count users in this unit
        var users = await _unitOfWork.Users.FindAsync(u => u.UnitId == id);
        var userCount = users.Count();

        return new UnitDto
        {
            Id = unit.Id,
            UnitCode = unit.Code,
            UnitName = unit.Name,
            UnitType = unit.Type,
            Description = unit.Description,
            ParentUnitId = unit.ParentUnitId,
            ParentUnitName = parentUnit?.Name,
            IsActive = unit.IsActive,
            ChildUnits = childUnits.Select(c => new UnitDto
            {
                Id = c.Id,
                UnitCode = c.Code,
                UnitName = c.Name,
                UnitType = c.Type,
                IsActive = c.IsActive,
                ChildUnits = new List<UnitDto>()
            }).ToList(),
            UserCount = userCount,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }

    public async Task<IEnumerable<UnitListDto>> GetByTypeAsync(string unitType)
    {
        var units = await _unitOfWork.Units.FindAsync(u => u.Type == unitType);
        var allUsers = await _unitOfWork.Users.GetAllAsync();

        return units.Select(u => new UnitListDto
        {
            Id = u.Id,
            UnitCode = u.Code,
            UnitName = u.Name,
            UnitType = u.Type,
            ParentUnitName = u.ParentUnit?.Name,
            UserCount = allUsers.Count(user => user.UnitId == u.Id),
            IsActive = u.IsActive
        }).OrderBy(u => u.UnitName);
    }

    public async Task<IEnumerable<UnitListDto>> GetChildUnitsAsync(int parentUnitId)
    {
        var units = await _unitOfWork.Units.FindAsync(u => u.ParentUnitId == parentUnitId);
        var allUsers = await _unitOfWork.Users.GetAllAsync();

        return units.Select(u => new UnitListDto
        {
            Id = u.Id,
            UnitCode = u.Code,
            UnitName = u.Name,
            UnitType = u.Type,
            ParentUnitName = u.ParentUnit?.Name,
            UserCount = allUsers.Count(user => user.UnitId == u.Id),
            IsActive = u.IsActive
        }).OrderBy(u => u.UnitName);
    }

    public async Task<IEnumerable<UnitDto>> GetHierarchyAsync()
    {
        var allUnits = (await _unitOfWork.Units.GetAllAsync()).ToList();
        var allUsers = await _unitOfWork.Users.GetAllAsync();

        // Get root units (no parent)
        var rootUnits = allUnits.Where(u => !u.ParentUnitId.HasValue).ToList();

        return rootUnits.Select(root => BuildUnitHierarchy(root, allUnits, allUsers));
    }

    private UnitDto BuildUnitHierarchy(Unit unit, List<Unit> allUnits, IEnumerable<AppUser> allUsers)
    {
        var childUnits = allUnits.Where(u => u.ParentUnitId == unit.Id).ToList();

        return new UnitDto
        {
            Id = unit.Id,
            UnitCode = unit.Code,
            UnitName = unit.Name,
            UnitType = unit.Type,
            Description = unit.Description,
            ParentUnitId = unit.ParentUnitId,
            IsActive = unit.IsActive,
            ChildUnits = childUnits.Select(c => BuildUnitHierarchy(c, allUnits, allUsers)).ToList(),
            UserCount = allUsers.Count(u => u.UnitId == unit.Id),
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }

    public async Task<bool> UnitCodeExistsAsync(string unitCode, int? excludeUnitId = null)
    {
        var units = await _unitOfWork.Units
            .FindAsync(u => u.Code.ToLower() == unitCode.ToLower() && !u.IsDeleted);

        if (excludeUnitId.HasValue)
        {
            units = units.Where(u => u.Id != excludeUnitId.Value);
        }

        return units.Any();
    }

    public async Task<UnitDto> CreateAsync(UnitCreateDto dto)
    {
        var unitCode = dto.UnitCode;
        if (string.IsNullOrWhiteSpace(unitCode))
        {
            unitCode = await GenerateUnitCodeAsync();
        }
        else
        {
            // Check if unit code already exists
            if (await UnitCodeExistsAsync(unitCode))
            {
                throw new InvalidOperationException($"Ma don vi '{unitCode}' da ton tai");
            }
        }

        // Validate ParentUnitId if provided
        if (dto.ParentUnitId.HasValue)
        {
            var parentUnit = await _unitOfWork.Units.GetByIdAsync(dto.ParentUnitId.Value);
            if (parentUnit == null)
            {
                throw new KeyNotFoundException($"Khong tim thay don vi cha voi ID {dto.ParentUnitId.Value}");
            }
        }

        var unit = new Unit
        {
            Code = unitCode,
            Name = dto.UnitName,
            Type = dto.UnitType,
            Description = dto.Description,
            ParentUnitId = dto.ParentUnitId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Units.AddAsync(unit);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(unit.Id))!;
    }

    private async Task<string> GenerateUnitCodeAsync()
    {
        var units = await _unitOfWork.Units.GetAllAsync();
        var maxNumber = 0;

        foreach (var unit in units)
        {
            if (unit.Code.StartsWith("UNIT-", StringComparison.OrdinalIgnoreCase))
            {
                var numPart = unit.Code.Substring(5);
                if (int.TryParse(numPart, out var num) && num > maxNumber)
                {
                    maxNumber = num;
                }
            }
        }

        return $"UNIT-{(maxNumber + 1):D3}";
    }

    public async Task<UnitDto> UpdateAsync(int id, UnitUpdateDto dto)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(id);
        if (unit == null)
        {
            throw new KeyNotFoundException($"Khong tim thay don vi voi ID {id}");
        }

        // Validate ParentUnitId if provided
        if (dto.ParentUnitId.HasValue)
        {
            if (dto.ParentUnitId.Value == id)
            {
                throw new InvalidOperationException("Don vi khong the la cha cua chinh no");
            }

            var parentUnit = await _unitOfWork.Units.GetByIdAsync(dto.ParentUnitId.Value);
            if (parentUnit == null)
            {
                throw new KeyNotFoundException($"Khong tim thay don vi cha voi ID {dto.ParentUnitId.Value}");
            }

            // Check for circular reference
            if (await IsCircularReference(id, dto.ParentUnitId.Value))
            {
                throw new InvalidOperationException("Khong the tao quan he cha-con vong tron");
            }
        }

        // Update unit properties
        unit.Name = dto.UnitName;
        unit.Type = dto.UnitType;
        unit.Description = dto.Description;
        unit.ParentUnitId = dto.ParentUnitId;
        unit.IsActive = dto.IsActive;
        unit.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Units.Update(unit);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(id);
        if (unit == null)
        {
            throw new KeyNotFoundException($"Khong tim thay don vi voi ID {id}");
        }

        // Check if unit has users
        var users = await _unitOfWork.Users.FindAsync(u => u.UnitId == id);
        if (users.Any())
        {
            throw new InvalidOperationException("Khong the xoa don vi dang co nguoi dung. Vui long chuyen nguoi dung sang don vi khac truoc.");
        }

        // Check if unit has child units
        var childUnits = await _unitOfWork.Units.FindAsync(u => u.ParentUnitId == id);
        if (childUnits.Any())
        {
            throw new InvalidOperationException("Khong the xoa don vi dang co don vi con. Vui long xoa hoac chuyen don vi con truoc.");
        }

        _unitOfWork.Units.Remove(unit);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task<bool> IsCircularReference(int unitId, int parentUnitId)
    {
        int? currentParentId = parentUnitId;
        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == unitId)
            {
                return true; // Circular reference detected
            }

            var parentUnit = await _unitOfWork.Units.GetByIdAsync(currentParentId.Value);
            currentParentId = parentUnit?.ParentUnitId;
        }

        return false;
    }
}
