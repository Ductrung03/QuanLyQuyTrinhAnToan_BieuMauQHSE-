using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SSMS.API.Controllers;
using SSMS.Application.DTOs;
using SSMS.Application.Services;
using System.Reflection;
using Xunit;

namespace SSMS.UnitTests.Controllers;

/// <summary>
/// Unit tests for RolesController - TDD Approach
/// Tests all 8 endpoints: GetAll, GetById, GetByCode, Create, Update, Delete, AssignPermissions, GetRolePermissions
/// </summary>
public class RolesControllerTests
{
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IAuditLogService> _mockAuditLogService;
    private readonly Mock<ILogger<RolesController>> _mockLogger;
    private readonly RolesController _controller;

    public RolesControllerTests()
    {
        _mockRoleService = new Mock<IRoleService>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _mockLogger = new Mock<ILogger<RolesController>>();
        _controller = new RolesController(_mockRoleService.Object, _mockAuditLogService.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Helper method to extract property from anonymous object or ApiResponse (case-insensitive)
    /// </summary>
    private static T GetPropertyValue<T>(object obj, string propertyName)
    {
        // Try exact match first
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        // If not found, try case-insensitive match (for ApiResponse with PascalCase properties)
        if (property == null)
        {
            property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        if (property == null)
            throw new InvalidOperationException($"Property '{propertyName}' not found on object of type {obj.GetType().Name}");
        return (T)property.GetValue(obj)!;
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithAllRoles()
    {
        // Arrange
        var roles = new List<RoleDto>
        {
            new RoleDto { Id = 1, Code = "ADMIN", Name = "Administrator", IsSystemRole = true },
            new RoleDto { Id = 2, Code = "MANAGER", Name = "Manager", IsSystemRole = true },
            new RoleDto { Id = 3, Code = "USER", Name = "User", IsSystemRole = true }
        };

        _mockRoleService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        var data = GetPropertyValue<IEnumerable<RoleDto>>(value, "data");
        data.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_ServiceThrows_Returns500()
    {
        // Arrange
        _mockRoleService.Setup(s => s.GetAllAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var value = objectResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
        GetPropertyValue<string>(value, "error").Should().NotBeNullOrEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_RoleExists_ReturnsOk()
    {
        // Arrange
        var roleId = 1;
        var role = new RoleDto
        {
            Id = roleId,
            Code = "ADMIN",
            Name = "Administrator",
            IsSystemRole = true
        };

        _mockRoleService.Setup(s => s.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.GetById(roleId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<RoleDto>(value, "data").Id.Should().Be(roleId);
    }

    [Fact]
    public async Task GetById_RoleNotFound_Returns404()
    {
        // Arrange
        var roleId = 999;

        _mockRoleService.Setup(s => s.GetByIdAsync(roleId))
            .ReturnsAsync((RoleDto?)null);

        // Act
        var result = await _controller.GetById(roleId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var value = notFoundResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
        GetPropertyValue<string>(value, "error").Should().Contain("999");
    }

    [Fact]
    public async Task GetById_ServiceThrows_Returns500()
    {
        // Arrange
        var roleId = 1;
        _mockRoleService.Setup(s => s.GetByIdAsync(roleId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetById(roleId);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    #endregion

    #region GetByCode Tests

    [Fact]
    public async Task GetByCode_RoleExists_ReturnsOk()
    {
        // Arrange
        var code = "ADMIN";
        var role = new RoleDto
        {
            Id = 1,
            Code = code,
            Name = "Administrator",
            IsSystemRole = true
        };

        _mockRoleService.Setup(s => s.GetByCodeAsync(code))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.GetByCode(code);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<RoleDto>(value, "data").Code.Should().Be(code);
    }

    [Fact]
    public async Task GetByCode_RoleNotFound_Returns404()
    {
        // Arrange
        var code = "NONEXISTENT";

        _mockRoleService.Setup(s => s.GetByCodeAsync(code))
            .ReturnsAsync((RoleDto?)null);

        // Act
        var result = await _controller.GetByCode(code);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var value = notFoundResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
        GetPropertyValue<string>(value, "error").Should().Contain("NONEXISTENT");
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ValidData_Returns201Created()
    {
        // Arrange
        var createDto = new RoleCreateDto
        {
            Code = "SUPERVISOR",
            Name = "Supervisor",
            Description = "Supervisor role"
        };

        var createdRole = new RoleDto
        {
            Id = 4,
            Code = "SUPERVISOR",
            Name = "Supervisor",
            Description = "Supervisor role",
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRoleService.Setup(s => s.CreateAsync(createDto))
            .ReturnsAsync(createdRole);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.ActionName.Should().Be(nameof(RolesController.GetById));

        var value = createdResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<RoleDto>(value, "data").Id.Should().Be(4);
    }

    [Fact]
    public async Task Create_DuplicateCode_Returns400BadRequest()
    {
        // Arrange
        var createDto = new RoleCreateDto
        {
            Code = "ADMIN",
            Name = "Another Admin"
        };

        _mockRoleService.Setup(s => s.CreateAsync(createDto))
            .ThrowsAsync(new InvalidOperationException("Role with code 'ADMIN' already exists"));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var value = badRequestResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
    }

    [Fact]
    public async Task Create_ServiceThrows_Returns500()
    {
        // Arrange
        var createDto = new RoleCreateDto
        {
            Code = "NEW_ROLE",
            Name = "New Role"
        };

        _mockRoleService.Setup(s => s.CreateAsync(createDto))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ValidData_ReturnsOk()
    {
        // Arrange
        var roleId = 4;
        var updateDto = new RoleUpdateDto
        {
            Name = "Updated Supervisor",
            Description = "Updated description"
        };

        var updatedRole = new RoleDto
        {
            Id = roleId,
            Code = "SUPERVISOR",
            Name = "Updated Supervisor",
            Description = "Updated description",
            IsSystemRole = false
        };

        _mockRoleService.Setup(s => s.UpdateAsync(roleId, updateDto))
            .ReturnsAsync(updatedRole);

        // Act
        var result = await _controller.Update(roleId, updateDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<RoleDto>(value, "data").Name.Should().Be("Updated Supervisor");
    }

    [Fact]
    public async Task Update_RoleNotFound_Returns404()
    {
        // Arrange
        var roleId = 999;
        var updateDto = new RoleUpdateDto { Name = "Updated Name" };

        _mockRoleService.Setup(s => s.UpdateAsync(roleId, updateDto))
            .ThrowsAsync(new KeyNotFoundException($"Role with ID {roleId} not found"));

        // Act
        var result = await _controller.Update(roleId, updateDto);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Update_InvalidOperation_Returns400()
    {
        // Arrange
        var roleId = 1;
        var updateDto = new RoleUpdateDto { Name = "Updated Admin" };

        _mockRoleService.Setup(s => s.UpdateAsync(roleId, updateDto))
            .ThrowsAsync(new InvalidOperationException("Cannot modify system role"));

        // Act
        var result = await _controller.Update(roleId, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidRole_Returns204NoContent()
    {
        // Arrange
        var roleId = 4;

        _mockRoleService.Setup(s => s.DeleteAsync(roleId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(roleId);

        // Assert
        var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task Delete_RoleNotFound_Returns404()
    {
        // Arrange
        var roleId = 999;

        _mockRoleService.Setup(s => s.DeleteAsync(roleId))
            .ThrowsAsync(new KeyNotFoundException($"Role with ID {roleId} not found"));

        // Act
        var result = await _controller.Delete(roleId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Delete_SystemRole_Returns400()
    {
        // Arrange
        var roleId = 1; // System Admin role

        _mockRoleService.Setup(s => s.DeleteAsync(roleId))
            .ThrowsAsync(new InvalidOperationException("Cannot delete system role"));

        // Act
        var result = await _controller.Delete(roleId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region AssignPermissions Tests

    [Fact]
    public async Task AssignPermissions_ValidData_ReturnsOk()
    {
        // Arrange
        var roleId = 2;
        var dto = new AssignPermissionsDto
        {
            PermissionIds = new[] { 1, 2, 3 }
        };

        _mockRoleService.Setup(s => s.AssignPermissionsAsync(roleId, dto.PermissionIds))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AssignPermissions(roleId, dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<string>(value, "message").Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AssignPermissions_RoleNotFound_Returns404()
    {
        // Arrange
        var roleId = 999;
        var dto = new AssignPermissionsDto { PermissionIds = new[] { 1, 2 } };

        _mockRoleService.Setup(s => s.AssignPermissionsAsync(roleId, dto.PermissionIds))
            .ThrowsAsync(new KeyNotFoundException($"Role with ID {roleId} not found"));

        // Act
        var result = await _controller.AssignPermissions(roleId, dto);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task AssignPermissions_ServiceThrows_Returns500()
    {
        // Arrange
        var roleId = 2;
        var dto = new AssignPermissionsDto { PermissionIds = new[] { 1, 2 } };

        _mockRoleService.Setup(s => s.AssignPermissionsAsync(roleId, dto.PermissionIds))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.AssignPermissions(roleId, dto);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    #endregion

    #region GetRolePermissions Tests

    [Fact]
    public async Task GetRolePermissions_ReturnsPermissions()
    {
        // Arrange
        var roleId = 2;
        var permissions = new List<PermissionDto>
        {
            new PermissionDto { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" },
            new PermissionDto { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" }
        };

        _mockRoleService.Setup(s => s.GetRolePermissionsAsync(roleId))
            .ReturnsAsync(permissions);

        // Act
        var result = await _controller.GetRolePermissions(roleId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        var data = GetPropertyValue<IEnumerable<PermissionDto>>(value, "data");
        data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRolePermissions_RoleNotFound_Returns404()
    {
        // Arrange
        var roleId = 999;

        _mockRoleService.Setup(s => s.GetRolePermissionsAsync(roleId))
            .ThrowsAsync(new KeyNotFoundException($"Role with ID {roleId} not found"));

        // Act
        var result = await _controller.GetRolePermissions(roleId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion
}
