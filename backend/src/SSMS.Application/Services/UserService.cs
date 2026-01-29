using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SSMS.Application.Common;
using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service implementation for User management
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<UserListDto>> GetPagedAsync(UserQueryParams queryParams)
    {
        var query = _unitOfWork.Users.GetQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            var search = queryParams.Search.ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(search) ||
                (u.FullName != null && u.FullName.ToLower().Contains(search)) ||
                (u.Email != null && u.Email.ToLower().Contains(search))
            );
        }

        if (queryParams.UnitId.HasValue)
        {
            query = query.Where(u => u.UnitId == queryParams.UnitId.Value);
        }

        if (!string.IsNullOrEmpty(queryParams.Role))
        {
            query = query.Where(u => u.Role == queryParams.Role);
        }

        if (queryParams.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == queryParams.IsActive.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = queryParams.SortDesc
            ? query.OrderByDescending(u => EF.Property<object>(u, queryParams.SortBy ?? "CreatedAt"))
            : query.OrderBy(u => EF.Property<object>(u, queryParams.SortBy ?? "CreatedAt"));

        // Apply pagination
        var users = await query
            .Skip(queryParams.Skip)
            .Take(queryParams.Take)
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Position = u.Position,
                Role = u.Role,
                UnitName = u.Unit != null ? u.Unit.Name : null,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return PagedResult<UserListDto>.Create(users, totalCount, queryParams.Page, queryParams.PageSize);
    }

    public async Task<IEnumerable<UserListDto>> GetAllAsync()
    {
        var users = await _unitOfWork.Users
            .FindAsync(u => u.IsActive);

        return users.Select(u => new UserListDto
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Position = u.Position,
            Role = u.Role,
            UnitName = u.Unit?.Name,
            IsActive = u.IsActive
        });
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return null;

        // Load Unit if needed
        Unit? unit = null;
        if (user.UnitId.HasValue)
        {
            unit = await _unitOfWork.Units.GetByIdAsync(user.UnitId.Value);
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.PhoneNumber,
            Position = user.Position,
            Role = user.Role,
            UnitId = user.UnitId,
            UnitName = unit?.Name,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<IEnumerable<UserListDto>> GetByUnitIdAsync(int unitId)
    {
        var users = await _unitOfWork.Users
            .FindAsync(u => u.UnitId == unitId && u.IsActive);

        return users.Select(u => new UserListDto
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Position = u.Position,
            Role = u.Role,
            UnitName = u.Unit?.Name,
            IsActive = u.IsActive
        });
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
    {
        var users = await _unitOfWork.Users
            .FindAsync(u =>
                u.Username.ToLower() == username.ToLower() &&
                u.IsActive &&
                !u.IsDeleted);

        if (excludeUserId.HasValue)
        {
            users = users.Where(u => u.Id != excludeUserId.Value);
        }

        return users.Any();
    }

    public async Task<UserDto> CreateAsync(UserCreateDto dto)
    {
        // Check if username already exists
        if (await UsernameExistsAsync(dto.Username))
        {
            throw new InvalidOperationException($"Tên đăng nhập '{dto.Username}' đã tồn tại");
        }

        // Validate UnitId if provided
        if (dto.UnitId.HasValue)
        {
            var unit = await _unitOfWork.Units.GetByIdAsync(dto.UnitId.Value);
            if (unit == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đơn vị với ID {dto.UnitId.Value}");
            }
        }

        var user = new AppUser
        {
            Username = dto.Username,
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.Phone,
            Position = dto.Position,
            Role = dto.Role,
            UnitId = dto.UnitId,
            IsActive = dto.IsActive,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(user.Id))!;
    }

    public async Task<UserDto> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID {id}");
        }

        // Validate UnitId if provided
        if (dto.UnitId.HasValue)
        {
            var unit = await _unitOfWork.Units.GetByIdAsync(dto.UnitId.Value);
            if (unit == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đơn vị với ID {dto.UnitId.Value}");
            }
        }

        // Update user properties (mutable approach for EF Core)
        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.Phone;
        user.Position = dto.Position;
        user.Role = dto.Role;
        user.UnitId = dto.UnitId;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID {id}");
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ReactivateAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID {id}");
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
