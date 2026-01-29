using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SSMS.API.Controllers;
using SSMS.Application.DTOs;
using SSMS.Application.Services;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace SSMS.UnitTests.Controllers;

/// <summary>
/// Unit tests for PermissionsController - TDD Approach
/// Tests all 3 endpoints: GetAll, GetGroupedByModule, CheckPermission
/// </summary>
public class PermissionsControllerTests
{
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<ILogger<PermissionsController>> _mockLogger;
    private readonly PermissionsController _controller;

    public PermissionsControllerTests()
    {
        _mockPermissionService = new Mock<IPermissionService>();
        _mockLogger = new Mock<ILogger<PermissionsController>>();
        _controller = new PermissionsController(_mockPermissionService.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Helper method to extract property from anonymous object
    /// </summary>
    private static T GetPropertyValue<T>(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
            throw new InvalidOperationException($"Property '{propertyName}' not found on object of type {obj.GetType().Name}");
        return (T)property.GetValue(obj)!;
    }

    private void SetupUserContext(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "testuser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithAllPermissions()
    {
        // Arrange
        var permissions = new List<PermissionDto>
        {
            new PermissionDto { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" },
            new PermissionDto { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" },
            new PermissionDto { Id = 3, Code = "tpl.view", Name = "View Template", Module = "Templates" }
        };

        _mockPermissionService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(permissions);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        var data = GetPropertyValue<IEnumerable<PermissionDto>>(value, "data");
        data.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_ServiceThrows_Returns500()
    {
        // Arrange
        _mockPermissionService.Setup(s => s.GetAllAsync())
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

    [Fact]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyArray()
    {
        // Arrange
        _mockPermissionService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Array.Empty<PermissionDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        var data = GetPropertyValue<IEnumerable<PermissionDto>>(value, "data");
        data.Should().BeEmpty();
    }

    #endregion

    #region GetGroupedByModule Tests

    [Fact]
    public async Task GetGroupedByModule_ReturnsOkWithGroupedPermissions()
    {
        // Arrange
        var groupedPermissions = new List<PermissionGroupDto>
        {
            new PermissionGroupDto
            {
                Module = "Procedures",
                Permissions = new List<PermissionDto>
                {
                    new PermissionDto { Id = 1, Code = "proc.view", Name = "View Procedure", Module = "Procedures" },
                    new PermissionDto { Id = 2, Code = "proc.create", Name = "Create Procedure", Module = "Procedures" }
                }
            },
            new PermissionGroupDto
            {
                Module = "Templates",
                Permissions = new List<PermissionDto>
                {
                    new PermissionDto { Id = 3, Code = "tpl.view", Name = "View Template", Module = "Templates" }
                }
            }
        };

        _mockPermissionService.Setup(s => s.GetGroupedByModuleAsync())
            .ReturnsAsync(groupedPermissions);

        // Act
        var result = await _controller.GetGroupedByModule();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        var data = GetPropertyValue<IEnumerable<PermissionGroupDto>>(value, "data");
        data.Should().HaveCount(2);
        data.First().Module.Should().Be("Procedures");
    }

    [Fact]
    public async Task GetGroupedByModule_ServiceThrows_Returns500()
    {
        // Arrange
        _mockPermissionService.Setup(s => s.GetGroupedByModuleAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetGroupedByModule();

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var value = objectResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
    }

    [Fact]
    public async Task GetGroupedByModule_EmptyList_ReturnsOkWithEmptyArray()
    {
        // Arrange
        _mockPermissionService.Setup(s => s.GetGroupedByModuleAsync())
            .ReturnsAsync(Array.Empty<PermissionGroupDto>());

        // Act
        var result = await _controller.GetGroupedByModule();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        var data = GetPropertyValue<IEnumerable<PermissionGroupDto>>(value, "data");
        data.Should().BeEmpty();
    }

    #endregion

    #region CheckPermission Tests

    [Fact]
    public async Task CheckPermission_UserHasPermission_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "proc.view";
        SetupUserContext(userId);

        _mockPermissionService.Setup(s => s.HasPermissionAsync(userId, permissionCode))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckPermission(permissionCode);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<bool>(value, "data").Should().BeTrue();
    }

    [Fact]
    public async Task CheckPermission_UserDoesNotHavePermission_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "admin.delete";
        SetupUserContext(userId);

        _mockPermissionService.Setup(s => s.HasPermissionAsync(userId, permissionCode))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CheckPermission(permissionCode);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var value = okResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeTrue();
        GetPropertyValue<bool>(value, "data").Should().BeFalse();
    }

    [Fact]
    public async Task CheckPermission_NoUserClaim_Returns401()
    {
        // Arrange - No user context set
        var permissionCode = "proc.view";

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.CheckPermission(permissionCode);

        // Assert
        var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);

        var value = unauthorizedResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
    }

    [Fact]
    public async Task CheckPermission_InvalidUserId_Returns401()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-id"), // Not a valid int
            new Claim(ClaimTypes.Name, "testuser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var permissionCode = "proc.view";

        // Act
        var result = await _controller.CheckPermission(permissionCode);

        // Assert
        var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task CheckPermission_ServiceThrows_Returns500()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "proc.view";
        SetupUserContext(userId);

        _mockPermissionService.Setup(s => s.HasPermissionAsync(userId, permissionCode))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CheckPermission(permissionCode);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var value = objectResult.Value!;
        GetPropertyValue<bool>(value, "success").Should().BeFalse();
    }

    #endregion
}
