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
/// Unit tests for PermissionService - TDD Approach
/// </summary>
public class PermissionServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<AppUser>> _mockUserRepo;
    private readonly Mock<IRepository<Role>> _mockRoleRepo;
    private readonly Mock<IRepository<Permission>> _mockPermissionRepo;
    private readonly Mock<IRepository<RolePermission>> _mockRolePermissionRepo;
    private readonly Mock<IRepository<UserPermission>> _mockUserPermissionRepo;
    private readonly IPermissionService _service;

    public PermissionServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IRepository<AppUser>>();
        _mockRoleRepo = new Mock<IRepository<Role>>();
        _mockPermissionRepo = new Mock<IRepository<Permission>>();
        _mockRolePermissionRepo = new Mock<IRepository<RolePermission>>();
        _mockUserPermissionRepo = new Mock<IRepository<UserPermission>>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<Role>()).Returns(_mockRoleRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<Permission>()).Returns(_mockPermissionRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<RolePermission>()).Returns(_mockRolePermissionRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<UserPermission>()).Returns(_mockUserPermissionRepo.Object);

        _service = new PermissionService(_mockUnitOfWork.Object);
    }

    #region HasPermissionAsync Tests

    [Fact]
    public async Task HasPermissionAsync_UserHasRolePermission_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "proc.create";

        var permission = new Permission
        {
            Id = 1,
            Code = permissionCode,
            Name = "Create Procedure",
            Module = "Procedures"
        };

        var role = new Role
        {
            Id = 2,
            Code = "manager",
            Name = "Manager",
            RolePermissions = new List<RolePermission>
            {
                new RolePermission
                {
                    RoleId = 2,
                    PermissionId = 1,
                    Permission = permission
                }
            }
        };

        var user = new AppUser
        {
            Id = userId,
            Username = "manager1",
            RoleId = 2,
            RoleEntity = role
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<UserPermission>().AsQueryable().BuildMock());

        // Act
        var result = await _service.HasPermissionAsync(userId, permissionCode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasPermissionAsync_UserLacksPermission_ReturnsFalse()
    {
        // Arrange
        var userId = 3;
        var permissionCode = "proc.create";

        var role = new Role
        {
            Id = 3,
            Code = "user",
            Name = "User",
            RolePermissions = new List<RolePermission>() // No permissions
        };

        var user = new AppUser
        {
            Id = userId,
            Username = "user1",
            RoleId = 3,
            RoleEntity = role
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<UserPermission>().AsQueryable().BuildMock());

        // Act
        var result = await _service.HasPermissionAsync(userId, permissionCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasPermissionAsync_UserOverrideGranted_ReturnsTrue()
    {
        // Arrange
        var userId = 3;
        var permissionCode = "proc.create";

        var permission = new Permission
        {
            Id = 1,
            Code = permissionCode,
            Name = "Create Procedure",
            Module = "Procedures"
        };

        var role = new Role
        {
            Id = 3,
            Code = "user",
            Name = "User",
            RolePermissions = new List<RolePermission>() // No role permissions
        };

        var user = new AppUser
        {
            Id = userId,
            Username = "user1",
            RoleId = 3,
            RoleEntity = role
        };

        var userPermission = new UserPermission
        {
            UserId = userId,
            PermissionId = 1,
            IsGranted = true,
            Permission = permission
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { userPermission }.AsQueryable().BuildMock());

        // Act
        var result = await _service.HasPermissionAsync(userId, permissionCode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasPermissionAsync_UserOverrideRevoked_ReturnsFalse()
    {
        // Arrange
        var userId = 2;
        var permissionCode = "proc.create";

        var permission = new Permission
        {
            Id = 1,
            Code = permissionCode,
            Name = "Create Procedure",
            Module = "Procedures"
        };

        var role = new Role
        {
            Id = 2,
            Code = "manager",
            Name = "Manager",
            RolePermissions = new List<RolePermission>
            {
                new RolePermission
                {
                    RoleId = 2,
                    PermissionId = 1,
                    Permission = permission
                }
            }
        };

        var user = new AppUser
        {
            Id = userId,
            Username = "manager1",
            RoleId = 2,
            RoleEntity = role
        };

        var userPermission = new UserPermission
        {
            UserId = userId,
            PermissionId = 1,
            IsGranted = false, // Revoked
            Permission = permission
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { userPermission }.AsQueryable().BuildMock());

        // Act
        var result = await _service.HasPermissionAsync(userId, permissionCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasPermissionAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var userId = 999;
        var permissionCode = "proc.create";

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<AppUser>().AsQueryable().BuildMock());

        // Act
        var result = await _service.HasPermissionAsync(userId, permissionCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasPermissionAsync_UserHasNoRole_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "proc.create";

        var user = new AppUser
        {
            Id = userId,
            Username = "user1",
            RoleId = null, // No role
            RoleEntity = null
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<UserPermission>().AsQueryable().BuildMock());

        // Act
        var result = await _service.HasPermissionAsync(userId, permissionCode);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetUserPermissionCodesAsync Tests

    [Fact]
    public async Task GetUserPermissionCodesAsync_ReturnsRolePermissions()
    {
        // Arrange
        var userId = 1;

        var permission1 = new Permission { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" };
        var permission2 = new Permission { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" };

        var role = new Role
        {
            Id = 2,
            Code = "manager",
            Name = "Manager",
            RolePermissions = new List<RolePermission>
            {
                new RolePermission { RoleId = 2, PermissionId = 1, Permission = permission1 },
                new RolePermission { RoleId = 2, PermissionId = 2, Permission = permission2 }
            }
        };

        var user = new AppUser
        {
            Id = userId,
            Username = "manager1",
            RoleId = 2,
            RoleEntity = role
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<UserPermission>().AsQueryable().BuildMock());

        // Act
        var result = await _service.GetUserPermissionCodesAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("proc.view");
        result.Should().Contain("proc.create");
    }

    [Fact]
    public async Task GetUserPermissionCodesAsync_MergesOverrides()
    {
        // Arrange
        var userId = 1;

        var permission1 = new Permission { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" };
        var permission2 = new Permission { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" };
        var permission3 = new Permission { Id = 3, Code = "proc.delete", Name = "Delete Procedure", Module = "Procedures" };

        var role = new Role
        {
            Id = 3,
            Code = "user",
            Name = "User",
            RolePermissions = new List<RolePermission>
            {
                new RolePermission { RoleId = 3, PermissionId = 1, Permission = permission1 },
                new RolePermission { RoleId = 3, PermissionId = 2, Permission = permission2 }
            }
        };

        var user = new AppUser
        {
            Id = userId,
            Username = "user1",
            RoleId = 3,
            RoleEntity = role
        };

        var userPermissions = new List<UserPermission>
        {
            new UserPermission { UserId = userId, PermissionId = 2, IsGranted = false, Permission = permission2 }, // Revoke proc.create
            new UserPermission { UserId = userId, PermissionId = 3, IsGranted = true, Permission = permission3 } // Grant proc.delete
        };

        _mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { user }.AsQueryable().BuildMock());

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(userPermissions.AsQueryable().BuildMock());

        // Act
        var result = await _service.GetUserPermissionCodesAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("proc.view"); // From role
        result.Should().NotContain("proc.create"); // Revoked by override
        result.Should().Contain("proc.delete"); // Granted by override
    }

    #endregion

    #region GrantPermissionToUserAsync Tests

    [Fact]
    public async Task GrantPermissionToUserAsync_CreatesNewOverride()
    {
        // Arrange
        var userId = 1;
        var permissionId = 5;

        var user = new AppUser { Id = userId, Username = "user1" };
        var permission = new Permission { Id = permissionId, Code = "proc.delete", Name = "Delete Procedure", Module = "Procedures" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockPermissionRepo.Setup(r => r.GetByIdAsync(permissionId))
            .ReturnsAsync(permission);

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<UserPermission>().AsQueryable().BuildMock());

        // Act
        await _service.GrantPermissionToUserAsync(userId, permissionId);

        // Assert
        _mockUserPermissionRepo.Verify(r => r.AddAsync(It.Is<UserPermission>(up =>
            up.UserId == userId &&
            up.PermissionId == permissionId &&
            up.IsGranted == true
        )), Times.Once);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GrantPermissionToUserAsync_UpdatesExistingOverride()
    {
        // Arrange
        var userId = 1;
        var permissionId = 5;

        var user = new AppUser { Id = userId, Username = "user1" };
        var permission = new Permission { Id = permissionId, Code = "proc.delete", Name = "Delete Procedure", Module = "Procedures" };

        var existingOverride = new UserPermission
        {
            UserId = userId,
            PermissionId = permissionId,
            IsGranted = false
        };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockPermissionRepo.Setup(r => r.GetByIdAsync(permissionId))
            .ReturnsAsync(permission);

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(new[] { existingOverride }.AsQueryable().BuildMock());

        // Act
        await _service.GrantPermissionToUserAsync(userId, permissionId);

        // Assert - After race condition fix: should Update instead of Remove+Add
        _mockUserPermissionRepo.Verify(r => r.Update(It.Is<UserPermission>(up =>
            up.UserId == userId &&
            up.PermissionId == permissionId &&
            up.IsGranted == true
        )), Times.Once);

        _mockUserPermissionRepo.Verify(r => r.Remove(It.IsAny<UserPermission>()), Times.Never);
        _mockUserPermissionRepo.Verify(r => r.AddAsync(It.IsAny<UserPermission>()), Times.Never);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GrantPermissionToUserAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        var userId = 999;
        var permissionId = 5;

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((AppUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GrantPermissionToUserAsync(userId, permissionId)
        );
    }

    [Fact]
    public async Task GrantPermissionToUserAsync_PermissionNotFound_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var permissionId = 999;

        var user = new AppUser { Id = userId, Username = "user1" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockPermissionRepo.Setup(r => r.GetByIdAsync(permissionId))
            .ReturnsAsync((Permission?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GrantPermissionToUserAsync(userId, permissionId)
        );
    }

    #endregion

    #region RevokePermissionFromUserAsync Tests

    [Fact]
    public async Task RevokePermissionFromUserAsync_CreatesRevokeOverride()
    {
        // Arrange
        var userId = 1;
        var permissionId = 2;

        var user = new AppUser { Id = userId, Username = "user1" };
        var permission = new Permission { Id = permissionId, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockPermissionRepo.Setup(r => r.GetByIdAsync(permissionId))
            .ReturnsAsync(permission);

        _mockUserPermissionRepo.Setup(r => r.GetQueryable())
            .Returns(Array.Empty<UserPermission>().AsQueryable().BuildMock());

        // Act
        await _service.RevokePermissionFromUserAsync(userId, permissionId);

        // Assert
        _mockUserPermissionRepo.Verify(r => r.AddAsync(It.Is<UserPermission>(up =>
            up.UserId == userId &&
            up.PermissionId == permissionId &&
            up.IsGranted == false
        )), Times.Once);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllPermissions()
    {
        // Arrange
        var permissions = new List<Permission>
        {
            new Permission { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" },
            new Permission { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" },
            new Permission { Id = 3, Code = "tpl.view", Name = "View Template", Module = "Templates" }
        };

        _mockPermissionRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(permissions);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Code == "proc.view");
        result.Should().Contain(p => p.Code == "proc.create");
        result.Should().Contain(p => p.Code == "tpl.view");
    }

    #endregion

    #region GetGroupedByModuleAsync Tests

    [Fact]
    public async Task GetGroupedByModuleAsync_GroupsPermissionsByModule()
    {
        // Arrange
        var permissions = new List<Permission>
        {
            new Permission { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" },
            new Permission { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" },
            new Permission { Id = 3, Code = "tpl.view", Name = "View Template", Module = "Templates" },
            new Permission { Id = 4, Code = "tpl.create", Name = "Create Template", Module = "Templates" }
        };

        _mockPermissionRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(permissions);

        // Act
        var result = await _service.GetGroupedByModuleAsync();

        // Assert
        result.Should().HaveCount(2);

        var procModule = result.FirstOrDefault(g => g.Module == "Procedures");
        procModule.Should().NotBeNull();
        procModule!.Permissions.Should().HaveCount(2);

        var tplModule = result.FirstOrDefault(g => g.Module == "Templates");
        tplModule.Should().NotBeNull();
        tplModule!.Permissions.Should().HaveCount(2);
    }

    #endregion
}
