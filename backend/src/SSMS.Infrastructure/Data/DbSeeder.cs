using Microsoft.EntityFrameworkCore;
using SSMS.Core.Entities;

namespace SSMS.Infrastructure.Data;

/// <summary>
/// Seed data cho database
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seed dữ liệu mẫu vào database
    /// </summary>
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Units
        var units = new[]
        {
            new Unit
            {
                Id = 1,
                Code = "HQ",
                Name = "Trụ sở chính",
                Type = "Headquarters",
                Description = "Văn phòng trụ sở chính",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Unit
            {
                Id = 2,
                Code = "SHIP001",
                Name = "Tàu Hải Phòng 01",
                Type = "Ship",
                Description = "Tàu khai thác số 1",
                ParentUnitId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Unit
            {
                Id = 3,
                Code = "SHIP002",
                Name = "Tàu Hải Phòng 02",
                Type = "Ship",
                Description = "Tàu khai thác số 2",
                ParentUnitId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Unit
            {
                Id = 4,
                Code = "DEPT-QHSE",
                Name = "Phòng QHSE",
                Type = "Department",
                Description = "Phòng Quản lý Chất lượng, An toàn và Môi trường",
                ParentUnitId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Unit
            {
                Id = 5,
                Code = "DEPT-OPS",
                Name = "Phòng Khai thác",
                Type = "Department",
                Description = "Phòng Khai thác và Vận hành",
                ParentUnitId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<Unit>().HasData(units);

        // Seed AppUsers
        var users = new[]
        {
            new AppUser
            {
                Id = 1,
                Username = "admin",
                Email = "admin@ssms.com",
                FullName = "Quản trị viên hệ thống",
                Role = "Admin",
                Position = "System Administrator",
                UnitId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 2,
                Username = "qhse.manager",
                Email = "qhse.manager@ssms.com",
                FullName = "Nguyễn Văn An",
                Role = "Manager",
                Position = "QHSE Manager",
                UnitId = 4,
                PhoneNumber = "0901234567",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 3,
                Username = "ops.manager",
                Email = "ops.manager@ssms.com",
                FullName = "Trần Thị Bình",
                Role = "Manager",
                Position = "Operations Manager",
                UnitId = 5,
                PhoneNumber = "0901234568",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 4,
                Username = "ship001.captain",
                Email = "captain.ship001@ssms.com",
                FullName = "Lê Văn Cường",
                Role = "User",
                Position = "Ship Captain",
                UnitId = 2,
                PhoneNumber = "0901234569",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 5,
                Username = "ship001.officer",
                Email = "officer.ship001@ssms.com",
                FullName = "Phạm Thị Dung",
                Role = "User",
                Position = "Safety Officer",
                UnitId = 2,
                PhoneNumber = "0901234570",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 6,
                Username = "ship002.captain",
                Email = "captain.ship002@ssms.com",
                FullName = "Hoàng Văn Em",
                Role = "User",
                Position = "Ship Captain",
                UnitId = 3,
                PhoneNumber = "0901234571",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 7,
                Username = "ship002.officer",
                Email = "officer.ship002@ssms.com",
                FullName = "Đỗ Thị Phương",
                Role = "User",
                Position = "Safety Officer",
                UnitId = 3,
                PhoneNumber = "0901234572",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 8,
                Username = "qhse.staff",
                Email = "qhse.staff@ssms.com",
                FullName = "Vũ Văn Giang",
                Role = "User",
                Position = "QHSE Staff",
                UnitId = 4,
                PhoneNumber = "0901234573",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<AppUser>().HasData(users);
    }
}
