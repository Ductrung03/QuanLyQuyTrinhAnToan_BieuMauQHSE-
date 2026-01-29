using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using SSMS.Application.DTOs;
using SSMS.Application.Services;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;
using Xunit;
using System.Linq.Expressions;

namespace SSMS.UnitTests.Services;

/// <summary>
/// Unit tests for RoleService - TDD Approach
/// </summary>
public class RoleServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Role>> _mockRoleRepo;
    private readonly Mock<IRepository<Permission>> _mockPermissionRepo;
    private readonly Mock<IRepository<RolePermission>> _mockRolePermissionRepo;
    private readonly IRoleService _service;

    public RoleServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoleRepo = new Mock<IRepository<Role>>();
        _mockPermissionRepo = new Mock<IRepository<Permission>>();
        _mockRolePermissionRepo = new Mock<IRepository<RolePermission>>();

        _mockUnitOfWork.Setup(u => u.GetRepository<Role>()).Returns(_mockRoleRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<Permission>()).Returns(_mockPermissionRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<RolePermission>()).Returns(_mockRolePermissionRepo.Object);

        _service = new RoleService(_mockUnitOfWork.Object);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_RoleExists_ReturnsRoleDto()
    {
        // Arrange
        var roleId = 1;
        var role = new Role
        {
            Id = roleId,
            Code = "admin",
            Name = "Administrator",
            Description = "System admin role",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act
        var result = await _service.GetByIdAsync(roleId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(roleId);
        result.Code.Should().Be("admin");
        result.Name.Should().Be("Administrator");
        result.IsSystemRole.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_RoleNotFound_ReturnsNull()
    {
        // Arrange
        var roleId = 999;

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync((Role?)null);

        // Act
        var result = await _service.GetByIdAsync(roleId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new Role { Id = 1, Code = "admin", Name = "Administrator", IsSystemRole = true },
            new Role { Id = 2, Code = "manager", Name = "Manager", IsSystemRole = true },
            new Role { Id = 3, Code = "user", Name = "User", IsSystemRole = true }
        };

        _mockRoleRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(r => r.Code == "admin");
        result.Should().Contain(r => r.Code == "manager");
        result.Should().Contain(r => r.Code == "user");
    }

    #endregion

    #region GetByCodeAsync Tests

    [Fact]
    public async Task GetByCodeAsync_RoleExists_ReturnsRoleDto()
    {
        // Arrange
        var roleCode = "manager";
        var role = new Role
        {
            Id = 2,
            Code = roleCode,
            Name = "Manager",
            IsSystemRole = true
        };

        _mockRoleRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(role);

        // Act
        var result = await _service.GetByCodeAsync(roleCode);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(roleCode);
    }

    [Fact]
    public async Task GetByCodeAsync_RoleNotFound_ReturnsNull()
    {
        // Arrange
        var roleCode = "nonexistent";

        _mockRoleRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync((Role?)null);

        // Act
        var result = await _service.GetByCodeAsync(roleCode);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidData_CreatesRole()
    {
        // Arrange
        var dto = new RoleCreateDto
        {
            Code = "supervisor",
            Name = "Supervisor",
            Description = "Supervisor role"
        };

        _mockRoleRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync((Role?)null);

        _mockRoleRepo.Setup(r => r.AddAsync(It.IsAny<Role>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("supervisor");
        result.Name.Should().Be("Supervisor");
        result.IsSystemRole.Should().BeFalse();

        _mockRoleRepo.Verify(r => r.AddAsync(It.Is<Role>(role =>
            role.Code == "supervisor" &&
            role.Name == "Supervisor" &&
            !role.IsSystemRole
        )), Times.Once);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ThrowsException()
    {
        // Arrange
        var dto = new RoleCreateDto
        {
            Code = "admin",
            Name = "Another Admin"
        };

        var existingRole = new Role { Id = 1, Code = "admin", Name = "Administrator" };

        _mockRoleRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .ReturnsAsync(existingRole);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto)
        );
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ValidData_UpdatesRole()
    {
        // Arrange
        var roleId = 4;
        var existingRole = new Role
        {
            Id = roleId,
            Code = "supervisor",
            Name = "Supervisor",
            Description = "Old description",
            IsSystemRole = false
        };

        var dto = new RoleUpdateDto
        {
            Name = "Updated Supervisor",
            Description = "New description"
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(existingRole);

        // Act
        var result = await _service.UpdateAsync(roleId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Supervisor");
        result.Description.Should().Be("New description");

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_RoleNotFound_ThrowsException()
    {
        // Arrange
        var roleId = 999;
        var dto = new RoleUpdateDto { Name = "Updated Name" };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateAsync(roleId, dto)
        );
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_NonSystemRole_DeletesSuccessfully()
    {
        // Arrange
        var roleId = 4;
        var role = new Role
        {
            Id = roleId,
            Code = "supervisor",
            Name = "Supervisor",
            IsSystemRole = false
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act
        await _service.DeleteAsync(roleId);

        // Assert
        _mockRoleRepo.Verify(r => r.Remove(role), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SystemRole_ThrowsException()
    {
        // Arrange
        var roleId = 1;
        var role = new Role
        {
            Id = roleId,
            Code = "admin",
            Name = "Administrator",
            IsSystemRole = true
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.DeleteAsync(roleId)
        );
    }

    [Fact]
    public async Task DeleteAsync_RoleNotFound_ThrowsException()
    {
        // Arrange
        var roleId = 999;

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.DeleteAsync(roleId)
        );
    }

    #endregion

    #region AssignPermissionsAsync Tests

    [Fact]
    public async Task AssignPermissionsAsync_ValidData_AssignsPermissions()
    {
        // Arrange
        var roleId = 2;
        var permissionIds = new List<int> { 1, 2, 3 };

        var role = new Role { Id = roleId, Code = "manager", Name = "Manager" };

        var permissions = new List<Permission>
        {
            new Permission { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" },
            new Permission { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" },
            new Permission { Id = 3, Code = "tpl.view", Name = "View Template", Module = "Templates" }
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        _mockPermissionRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(permissions);

        _mockRolePermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<RolePermission>().AsQueryable().BuildMock());

        // Act
        await _service.AssignPermissionsAsync(roleId, permissionIds);

        // Assert
        _mockRolePermissionRepo.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<RolePermission>>(rps =>
            rps.Count() == 3 &&
            rps.All(rp => rp.RoleId == roleId) &&
            rps.Select(rp => rp.PermissionId).OrderBy(id => id).SequenceEqual(new[] { 1, 2, 3 })
        )), Times.Once);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task AssignPermissionsAsync_RoleNotFound_ThrowsException()
    {
        // Arrange
        var roleId = 999;
        var permissionIds = new List<int> { 1, 2 };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.AssignPermissionsAsync(roleId, permissionIds)
        );
    }

    [Fact]
    public async Task AssignPermissionsAsync_RemovesOldPermissions()
    {
        // Arrange
        var roleId = 2;
        var newPermissionIds = new List<int> { 3, 4 };

        var role = new Role { Id = roleId, Code = "manager", Name = "Manager" };

        var existingRolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = roleId, PermissionId = 1 },
            new RolePermission { RoleId = roleId, PermissionId = 2 }
        };

        var newPermissions = new List<Permission>
        {
            new Permission { Id = 3, Code = "tpl.create", Name = "Create Template", Module = "Templates" },
            new Permission { Id = 4, Code = "tpl.edit", Name = "Edit Template", Module = "Templates" }
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        _mockRolePermissionRepo.Setup(r => r.GetQueryable())
            .Returns(existingRolePermissions.AsQueryable().BuildMock());

        _mockPermissionRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Permission, bool>>>()))
            .ReturnsAsync(newPermissions);

        // Act
        await _service.AssignPermissionsAsync(roleId, newPermissionIds);

        // Assert
        _mockRolePermissionRepo.Verify(r => r.RemoveRange(It.Is<IEnumerable<RolePermission>>(rps =>
            rps.Count() == 2 &&
            rps.Select(rp => rp.PermissionId).OrderBy(id => id).SequenceEqual(new[] { 1, 2 })
        )), Times.Once);

        _mockRolePermissionRepo.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<RolePermission>>(rps =>
            rps.Count() == 2 &&
            rps.Select(rp => rp.PermissionId).OrderBy(id => id).SequenceEqual(new[] { 3, 4 })
        )), Times.Once);
    }

    #endregion

    #region GetRolePermissionsAsync Tests

    [Fact]
    public async Task GetRolePermissionsAsync_ReturnsPermissions()
    {
        // Arrange
        var roleId = 2;
        var role = new Role { Id = roleId, Code = "manager", Name = "Manager", IsSystemRole = true };

        var permission1 = new Permission { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" };
        var permission2 = new Permission { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" };

        var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = roleId, PermissionId = 1, Permission = permission1 },
            new RolePermission { RoleId = roleId, PermissionId = 2, Permission = permission2 }
        };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        _mockRolePermissionRepo.Setup(r => r.GetQueryable())
            .Returns(rolePermissions.AsQueryable().BuildMock());

        // Act
        var result = await _service.GetRolePermissionsAsync(roleId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Code == "proc.view");
        result.Should().Contain(p => p.Code == "proc.create");
    }

    [Fact]
    public async Task GetRolePermissionsAsync_NoPermissions_ReturnsEmpty()
    {
        // Arrange
        var roleId = 3;
        var role = new Role { Id = roleId, Code = "user", Name = "User", IsSystemRole = true };

        _mockRoleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        _mockRolePermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<RolePermission>().AsQueryable().BuildMock());

        // Act
        var result = await _service.GetRolePermissionsAsync(roleId);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
