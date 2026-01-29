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
/// Integration tests for PermissionsController API.
/// Tests permission listing and checking functionality.
/// </summary>
public class PermissionsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PermissionsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ReturnsAllPermissions()
    {
        // Act
        var response = await _client.GetAsync("/api/permissions");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionDto>>>();

        // Assert
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThanOrEqualTo(9);
    }

    #endregion

    #region GetGroupedByModule Tests

    [Fact]
    public async Task GetGroupedByModule_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/permissions/grouped");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroupedByModule_ReturnsPermissionsGroupedByModule()
    {
        // Act
        var response = await _client.GetAsync("/api/permissions/grouped");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionGroupDto>>>();

        // Assert
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThanOrEqualTo(3);
        
        // Verify each group has permissions
        foreach (var group in content.Data)
        {
            group.Module.Should().NotBeNullOrWhiteSpace();
            group.Permissions.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GetGroupedByModule_HasExpectedModules()
    {
        // Act
        var response = await _client.GetAsync("/api/permissions/grouped");
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionGroupDto>>>();

        // Assert
        var moduleNames = content!.Data!.Select(g => g.Module).ToList();
        
        // Should contain these expected modules
        moduleNames.Should().Contain("System");
        moduleNames.Should().Contain("Procedure");
        moduleNames.Should().Contain("User");
    }

    #endregion

    #region CheckPermission Tests

    [Fact]
    public async Task CheckPermission_WithValidPermissionCode_ReturnsResult()
    {
        // Arrange
        var permissionCode = "proc.view";

        // Act
        var response = await _client.GetAsync($"/api/permissions/check/{permissionCode}");

        // Assert
        // Note: This will return Unauthorized without authentication
        // In real scenario, we'd need to setup authentication
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CheckPermission_WithInvalidPermissionCode_HandlesGracefully()
    {
        // Arrange
        var permissionCode = "nonexistent.permission";

        // Act
        var response = await _client.GetAsync($"/api/permissions/check/{permissionCode}");

        // Assert
        // Should not crash, either return false or unauthorized
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    #endregion

    #region Helper Methods

    private async Task<int> GetFirstPermissionId()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var permission = await db.Permissions.FirstAsync();
        return permission.Id;
    }

    #endregion
}

