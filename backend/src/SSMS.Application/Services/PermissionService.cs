using Microsoft.EntityFrameworkCore;
using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service implementation for Permission management
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;

    public PermissionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions = await _unitOfWork.GetRepository<Permission>().GetAllAsync();

        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Module = p.Module,
            Description = p.Description
        });
    }

    public async Task<IEnumerable<PermissionGroupDto>> GetGroupedByModuleAsync()
    {
        var permissions = await _unitOfWork.GetRepository<Permission>().GetAllAsync();

        var grouped = permissions
            .GroupBy(p => p.Module)
            .Select(g => new PermissionGroupDto
            {
                Module = g.Key,
                Permissions = g.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Module = p.Module,
                    Description = p.Description
                })
            });

        return grouped;
    }

    public async Task<IEnumerable<string>> GetUserPermissionCodesAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetQueryable()
            .Include(u => u.RoleEntity)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.RoleEntity == null)
        {
            return Array.Empty<string>();
        }

        var userOverrides = await _unitOfWork.GetRepository<UserPermission>().GetQueryable()
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .ToListAsync();

        var rolePermissionCodes = user.RoleEntity.RolePermissions
            .Select(rp => rp.Permission.Code)
            .ToHashSet();

        foreach (var userOverride in userOverrides)
        {
            if (userOverride.IsGranted)
            {
                rolePermissionCodes.Add(userOverride.Permission.Code);
            }
            else
            {
                rolePermissionCodes.Remove(userOverride.Permission.Code);
            }
        }

        return rolePermissionCodes;
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
    {
        var user = await _unitOfWork.Users.GetQueryable()
            .Include(u => u.RoleEntity)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var userOverride = await _unitOfWork.GetRepository<UserPermission>().GetQueryable()
            .Include(up => up.Permission)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.Permission.Code == permissionCode);

        if (userOverride != null)
        {
            return userOverride.IsGranted;
        }

        if (user.RoleEntity == null)
        {
            return false;
        }

        return user.RoleEntity.RolePermissions
            .Any(rp => rp.Permission.Code == permissionCode);
    }

    public async Task GrantPermissionToUserAsync(int userId, int permissionId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID {userId}");
        }

        var permission = await _unitOfWork.GetRepository<Permission>().GetByIdAsync(permissionId);
        if (permission == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy quyền với ID {permissionId}");
        }

        var existingOverride = await _unitOfWork.GetRepository<UserPermission>().GetQueryable()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existingOverride != null)
        {
            // UPDATE existing record instead of remove+add (prevents race condition)
            if (!existingOverride.IsGranted)
            {
                existingOverride.IsGranted = true;
                existingOverride.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<UserPermission>().Update(existingOverride);
                await _unitOfWork.SaveChangesAsync();
            }
            // Else: Already granted, no action needed
        }
        else
        {
            // INSERT new record
            var newOverride = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsGranted = true
            };

            await _unitOfWork.GetRepository<UserPermission>().AddAsync(newOverride);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RevokePermissionFromUserAsync(int userId, int permissionId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID {userId}");
        }

        var permission = await _unitOfWork.GetRepository<Permission>().GetByIdAsync(permissionId);
        if (permission == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy quyền với ID {permissionId}");
        }

        var existingOverride = await _unitOfWork.GetRepository<UserPermission>().GetQueryable()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existingOverride != null)
        {
            // UPDATE existing record instead of remove+add (prevents race condition)
            if (existingOverride.IsGranted)
            {
                existingOverride.IsGranted = false;
                existingOverride.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<UserPermission>().Update(existingOverride);
                await _unitOfWork.SaveChangesAsync();
            }
            // Else: Already revoked, no action needed
        }
        else
        {
            // INSERT new revoke record
            var newOverride = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsGranted = false
            };

            await _unitOfWork.GetRepository<UserPermission>().AddAsync(newOverride);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserPermissionDto>> GetUserPermissionOverridesAsync(int userId)
    {
        var overrides = await _unitOfWork.GetRepository<UserPermission>().GetQueryable()
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .ToListAsync();

        return overrides.Select(up => new UserPermissionDto
        {
            UserId = up.UserId,
            PermissionId = up.PermissionId,
            PermissionCode = up.Permission.Code,
            PermissionName = up.Permission.Name,
            IsGranted = up.IsGranted,
            CreatedAt = up.CreatedAt
        });
    }

    public async Task RemoveUserPermissionOverrideAsync(int userId, int permissionId)
    {
        var existingOverride = await _unitOfWork.GetRepository<UserPermission>().GetQueryable()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existingOverride != null)
        {
            _unitOfWork.GetRepository<UserPermission>().Remove(existingOverride);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
