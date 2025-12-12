using Microsoft.EntityFrameworkCore;
using SSMS.Core.Entities;

namespace SSMS.Infrastructure.Data;

/// <summary>
/// Database Context cho ứng dụng SSMS
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Unit> Units { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình cho AppUser
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AppUser");
            
            // Map Id -> UserId
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("UserId");

            // Map Username -> UserName (column đã có)
            entity.Property(e => e.Username)
                .HasColumnName("UserName")
                .IsRequired()
                .HasMaxLength(100);

            // FullName - column mới, sẽ thêm qua migration
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            // Email - column đã có
            entity.Property(e => e.Email)
                .HasMaxLength(255);

            // Map PhoneNumber -> Phone (column đã có)
            entity.Property(e => e.PhoneNumber)
                .HasColumnName("Phone")
                .HasMaxLength(50);

            // Position - column mới, sẽ thêm qua migration
            entity.Property(e => e.Position)
                .HasMaxLength(100);

            // Role - column mới, sẽ thêm qua migration
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("User");

            // UnitId - column mới, sẽ thêm qua migration
            entity.Property(e => e.UnitId);

            // PasswordHash - column mới (nullable)
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500);

            // LastLoginAt - column mới (nullable)
            entity.Property(e => e.LastLoginAt);

            // Relationship với Unit
            entity.HasOne(e => e.Unit)
                .WithMany(u => u.Users)
                .HasForeignKey(e => e.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime2(0)");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // IsActive - column đã có
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Query filter cho soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Cấu hình cho Unit
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Unit");
            
            // Map Id -> UnitId
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("UnitId");

            // Map Code -> UnitCode (column đã có)
            entity.Property(e => e.Code)
                .HasColumnName("UnitCode")
                .HasMaxLength(50);

            // Map Name -> UnitName (column đã có)
            entity.Property(e => e.Name)
                .HasColumnName("UnitName")
                .IsRequired()
                .HasMaxLength(200);

            // Map Type -> UnitType (column đã có)
            entity.Property(e => e.Type)
                .HasColumnName("UnitType")
                .HasMaxLength(50);

            // Description - column mới, sẽ thêm qua migration
            entity.Property(e => e.Description)
                .HasMaxLength(500);

            // ParentUnitId - column mới, sẽ thêm qua migration
            entity.Property(e => e.ParentUnitId);

            // Self-referencing relationship cho cấu trúc phân cấp
            entity.HasOne(e => e.ParentUnit)
                .WithMany(u => u.ChildUnits)
                .HasForeignKey(e => e.ParentUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime2(0)");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // IsActive - column đã có
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Query filter cho soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Seed initial data - DISABLED vì database đã có sẵn
        // DbSeeder.SeedData(modelBuilder);
    }

    /// <summary>
    /// Override SaveChanges để tự động cập nhật timestamps
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync để tự động cập nhật timestamps
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Tự động cập nhật CreatedAt và UpdatedAt
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
