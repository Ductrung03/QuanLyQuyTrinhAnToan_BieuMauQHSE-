using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSMS.Core.Entities;
using SSMS.Infrastructure.Data;

namespace SSMS.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder("Test")
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();
            
            SeedDatabase(db);
        });

        builder.UseEnvironment("Testing");
    }

    private void SeedDatabase(AppDbContext db)
    {
        if (db.Permissions.Any()) return;

        if (!db.Roles.Any())
        {
            var roles = new List<Role>
            {
                new Role { Name = "Admin", Code = "ADMIN", Description = "System Administrator", IsSystemRole = true },
                new Role { Name = "Manager", Code = "MANAGER", Description = "Manager", IsSystemRole = true },
                new Role { Name = "User", Code = "USER", Description = "Standard User", IsSystemRole = true },
                new Role { Name = "Captain", Code = "CAPTAIN", Description = "Ship Captain", IsSystemRole = true },
                new Role { Name = "Safety Officer", Code = "SAFETY_OFFICER", Description = "Safety Officer", IsSystemRole = true }
            };
            db.Roles.AddRange(roles);
        }

        var permissions = new List<Permission>
        {
            new Permission { Name = "System Manage", Code = "system.manage", Module = "System" },
            new Permission { Name = "User View", Code = "user.view", Module = "User" },
            new Permission { Name = "User Edit", Code = "user.edit", Module = "User" },
            new Permission { Name = "Procedure View", Code = "procedure.view", Module = "Procedure" },
            new Permission { Name = "Procedure Create", Code = "procedure.create", Module = "Procedure" },
            new Permission { Name = "Procedure Edit", Code = "procedure.edit", Module = "Procedure" },
            new Permission { Name = "Procedure Delete", Code = "procedure.delete", Module = "Procedure" },
            new Permission { Name = "Procedure Approve", Code = "procedure.approve", Module = "Procedure" },
            new Permission { Name = "Procedure Publish", Code = "procedure.publish", Module = "Procedure" }
        };
        db.Permissions.AddRange(permissions);

        db.SaveChanges();

        var adminRole = db.Roles.First(r => r.Code == "ADMIN");
        var allPermissions = db.Permissions.ToList();
        
        foreach (var perm in allPermissions)
        {
            db.Set<RolePermission>().Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = perm.Id
            });
        }
        db.SaveChanges();
    }
}
