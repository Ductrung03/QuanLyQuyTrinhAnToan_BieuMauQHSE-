using Microsoft.EntityFrameworkCore;
using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service implementation for Role management
/// </summary>
public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(id);
        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _unitOfWork.GetRepository<Role>().GetAllAsync();

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            Description = r.Description,
            IsSystemRole = r.IsSystemRole,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });
    }

    public async Task<RoleDto?> GetByCodeAsync(string code)
    {
        var role = await _unitOfWork.GetRepository<Role>()
            .FirstOrDefaultAsync(r => r.Code == code);

        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    public async Task<RoleDto> CreateAsync(RoleCreateDto dto)
    {
        var code = dto.Code;
        if (string.IsNullOrWhiteSpace(code))
        {
            code = await GenerateRoleCodeAsync();
        }
        else
        {
            var existingRole = await _unitOfWork.GetRepository<Role>()
                .FirstOrDefaultAsync(r => r.Code == code);

            if (existingRole != null)
            {
                throw new InvalidOperationException($"Vai trò với mã '{code}' đã tồn tại");
            }
        }

        var role = new Role
        {
            Name = dto.Name,
            Code = code,
            Description = dto.Description,
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<Role>().AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    private async Task<string> GenerateRoleCodeAsync()
    {
        var roles = await _unitOfWork.GetRepository<Role>().GetAllAsync();
        var maxNumber = 0;

        foreach (var role in roles)
        {
            if (role.Code.StartsWith("ROLE-", StringComparison.OrdinalIgnoreCase))
            {
                var numPart = role.Code.Substring(5);
                if (int.TryParse(numPart, out var num) && num > maxNumber)
                {
                    maxNumber = num;
                }
            }
        }

        return $"ROLE-{(maxNumber + 1):D3}";
    }

    public async Task<RoleDto> UpdateAsync(int id, RoleUpdateDto dto)
    {
        var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(id);
        if (role == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy vai trò với ID {id}");
        }

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.GetRepository<Role>().Update(role);
        await _unitOfWork.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(id);
        if (role == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy vai trò với ID {id}");
        }

        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("Không thể xóa vai trò hệ thống");
        }

        _unitOfWork.GetRepository<Role>().Remove(role);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy vai trò với ID {roleId}");
        }

        var permissions = await _unitOfWork.GetRepository<Permission>()
            .FindAsync(p => permissionIds.Contains(p.Id));

        if (permissions.Count() != permissionIds.Count())
        {
            throw new InvalidOperationException("Một số quyền không tồn tại");
        }

        // FIX HIGH-05: Use async version instead of synchronous .ToList()
        var existingRolePermissions = await _unitOfWork.GetRepository<RolePermission>().GetQueryable()
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

        _unitOfWork.GetRepository<RolePermission>().RemoveRange(existingRolePermissions);

        var newRolePermissions = permissionIds.Select(permissionId => new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.UtcNow
        });

        await _unitOfWork.GetRepository<RolePermission>().AddRangeAsync(newRolePermissions);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId)
    {
        var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy vai trò với ID {roleId}");
        }

        var rolePermissions = await _unitOfWork.GetRepository<RolePermission>().GetQueryable()
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

        return rolePermissions.Select(rp => new PermissionDto
        {
            Id = rp.Permission.Id,
            Name = rp.Permission.Name,
            Code = rp.Permission.Code,
            Module = rp.Permission.Module,
            Description = rp.Permission.Description
        });
    }
}
