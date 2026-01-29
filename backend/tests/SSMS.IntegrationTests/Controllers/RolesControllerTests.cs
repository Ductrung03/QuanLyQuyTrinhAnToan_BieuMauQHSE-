using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SSMS.Application.DTOs;
using SSMS.Infrastructure.Data;
using SSMS.IntegrationTests.Fixtures;
using SSMS.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SSMS.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for RolesController API.
/// Tests all CRUD operations and permission assignment functionality.
/// </summary>
public class RolesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RolesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ReturnsAllRoles()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/roles");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<RoleDto>>>();

        // Assert
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThanOrEqualTo(5); // At least 5 system roles
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsRole()
    {
        // Arrange
        await SeedTestData();
        var roleId = await GetFirstRoleId();

        // Act
        var response = await _client.GetAsync($"/api/roles/{roleId}");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RoleDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(roleId);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/roles/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GetByCode Tests

    [Fact]
    public async Task GetByCode_WithValidCode_ReturnsRole()
    {
        // Arrange
        await SeedTestData();
        var code = "ADMIN";

        // Act
        var response = await _client.GetAsync($"/api/roles/by-code/{code}");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RoleDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Code.Should().Be(code);
    }

    [Fact]
    public async Task GetByCode_WithInvalidCode_ReturnsNotFound()
    {
        // Arrange
        var invalidCode = "NONEXISTENT";

        // Act
        var response = await _client.GetAsync($"/api/roles/by-code/{invalidCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new RoleCreateDto
        {
            Name = "Test Role",
            Code = "TEST_ROLE",
            Description = "Test role for integration testing"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createDto);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RoleDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Name.Should().Be(createDto.Name);
        content.Data.Code.Should().Be(createDto.Code);
    }

    [Fact]
    public async Task Create_WithDuplicateCode_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestData();
        var createDto = new RoleCreateDto
        {
            Name = "Duplicate Test",
            Code = "ADMIN", // Already exists
            Description = "Should fail"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new RoleCreateDto
        {
            Name = "", // Invalid: empty name
            Code = "TEST",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await SeedTestData();
        var roleId = await CreateTestRole();
        var updateDto = new RoleUpdateDto
        {
            Name = "Updated Role Name",
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/roles/{roleId}", updateDto);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RoleDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Name.Should().Be(updateDto.Name);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;
        var updateDto = new RoleUpdateDto
        {
            Name = "Updated Name",
            Description = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/roles/{invalidId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_SystemRole_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestData();
        var adminRoleId = await GetRoleIdByCode("ADMIN");
        var updateDto = new RoleUpdateDto
        {
            Name = "Modified Admin",
            Description = "Should not be allowed"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/roles/{adminRoleId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        await SeedTestData();
        var roleId = await CreateTestRole();

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{roleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_SystemRole_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestData();
        var adminRoleId = await GetRoleIdByCode("ADMIN");

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{adminRoleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region AssignPermissions Tests

    [Fact]
    public async Task AssignPermissions_WithValidData_ReturnsSuccess()
    {
        // Arrange
        await SeedTestData();
        var roleId = await CreateTestRole();
        var permissionIds = await GetFirstThreePermissionIds();
        var assignDto = new AssignPermissionsDto
        {
            PermissionIds = permissionIds
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/roles/{roleId}/permissions", assignDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignPermissions_WithInvalidRoleId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;
        var assignDto = new AssignPermissionsDto
        {
            PermissionIds = new List<int> { 1, 2, 3 }
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/roles/{invalidId}/permissions", assignDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignPermissions_WithInvalidPermissionIds_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestData();
        var roleId = await CreateTestRole();
        var assignDto = new AssignPermissionsDto
        {
            PermissionIds = new List<int> { 99999, 99998 } // Non-existent permission IDs
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/roles/{roleId}/permissions", assignDto);

        // Assert
        // Should return error (either BadRequest or NotFound depending on implementation)
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    #endregion

    #region GetRolePermissions Tests

    [Fact]
    public async Task GetRolePermissions_WithValidId_ReturnsPermissions()
    {
        // Arrange
        await SeedTestData();
        var adminRoleId = await GetRoleIdByCode("ADMIN");

        // Act
        var response = await _client.GetAsync($"/api/roles/{adminRoleId}/permissions");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionDto>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThan(0); // Admin should have permissions
    }

    [Fact]
    public async Task GetRolePermissions_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/roles/{invalidId}/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

    private async Task SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Database is already seeded by migration
        await Task.CompletedTask;
    }

    private async Task<int> GetFirstRoleId()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var role = await db.Roles.FirstAsync();
        return role.Id;
    }

    private async Task<int> GetRoleIdByCode(string code)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var role = await db.Roles.FirstAsync(r => r.Code == code);
        return role.Id;
    }

    private async Task<int> CreateTestRole()
    {
        var createDto = new RoleCreateDto
        {
            Name = $"Test Role {Guid.NewGuid()}",
            Code = $"TEST_{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Description = "Temporary test role"
        };

        var response = await _client.PostAsJsonAsync("/api/roles", createDto);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RoleDto>>();
        return content!.Data!.Id;
    }

    private async Task<List<int>> GetFirstThreePermissionIds()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Permissions.Take(3).Select(p => p.Id).ToListAsync();
    }

    #endregion
}

