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
    public DbSet<OpsProcedure> OpsProcedures { get; set; }
    public DbSet<OpsProcedureDocument> OpsProcedureDocuments { get; set; }
    public DbSet<OpsTemplate> OpsTemplates { get; set; }
    public DbSet<OpsSubmission> OpsSubmissions { get; set; }
    public DbSet<OpsSubmissionFile> OpsSubmissionFiles { get; set; }
    public DbSet<OpsSubmissionRecipient> OpsSubmissionRecipients { get; set; }
    public DbSet<OpsApproval> OpsApprovals { get; set; }

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
                .IsRequired(false)
                .HasMaxLength(200);

            // Email - column đã có
            entity.Property(e => e.Email)
                .IsRequired(false)
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
                .IsRequired(false)
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
                .IsRequired(false)
                .HasMaxLength(50);

            // Map Name -> UnitName (column đã có)
            entity.Property(e => e.Name)
                .HasColumnName("UnitName")
                .IsRequired(false)
                .HasMaxLength(200);

            // Map Type -> UnitType (column đã có)
            entity.Property(e => e.Type)
                .HasColumnName("UnitType")
                .IsRequired(false)
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

        // Cấu hình cho OpsProcedure
        modelBuilder.Entity<OpsProcedure>(entity =>
        {
            entity.ToTable("OpsProcedure");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ProcedureId");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.Code)
                .IsUnique();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Version)
                .HasMaxLength(20);

            entity.Property(e => e.State)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Draft");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");

            // Relationships
            entity.HasOne(e => e.OwnerUser)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AuthorUser)
                .WithMany()
                .HasForeignKey(e => e.AuthorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ApproverUser)
                .WithMany()
                .HasForeignKey(e => e.ApproverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime2(0)");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Cấu hình cho OpsProcedureDocument
        modelBuilder.Entity<OpsProcedureDocument>(entity =>
        {
            entity.ToTable("OpsProcedureDocument");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ProcedureDocId");

            entity.Property(e => e.DocVersion)
                .HasMaxLength(20);

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FilePath)
                .HasMaxLength(500);

            entity.Property(e => e.ContentType)
                .HasMaxLength(100);

            entity.Property(e => e.UploadedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationship
            entity.HasOne(e => e.Procedure)
                .WithMany(p => p.Documents)
                .HasForeignKey(e => e.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Cấu hình cho OpsTemplate
        modelBuilder.Entity<OpsTemplate>(entity =>
        {
            entity.ToTable("OpsTemplate");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("TemplateId");

            entity.Property(e => e.TemplateKey)
                .HasMaxLength(32);

            entity.Property(e => e.TemplateNo)
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.TemplateType)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Form");

            entity.Property(e => e.State)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Draft");

            entity.Property(e => e.FileName)
                .HasMaxLength(255);

            entity.Property(e => e.FilePath)
                .HasMaxLength(500);

            entity.Property(e => e.ContentType)
                .HasMaxLength(100);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Relationship
            entity.HasOne(e => e.Procedure)
                .WithMany(p => p.Templates)
                .HasForeignKey(e => e.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);

            // Index
            entity.HasIndex(e => e.ProcedureId);
        });

        // Cấu hình cho OpsSubmission
        modelBuilder.Entity<OpsSubmission>(entity =>
        {
            entity.ToTable("OpsSubmission");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("SubmissionId");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Submitted");

            entity.Property(e => e.Content)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.RecallReason)
                .HasMaxLength(500);

            entity.Property(e => e.SubmittedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationships
            entity.HasOne(e => e.Procedure)
                .WithMany()
                .HasForeignKey(e => e.ProcedureId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Template)
                .WithMany()
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SubmittedByUser)
                .WithMany()
                .HasForeignKey(e => e.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);

            // Indexes
            entity.HasIndex(e => e.ProcedureId);
            entity.HasIndex(e => e.SubmittedByUserId);
            entity.HasIndex(e => e.Status);
        });

        // Cấu hình cho OpsSubmissionFile
        modelBuilder.Entity<OpsSubmissionFile>(entity =>
        {
            entity.ToTable("OpsSubmissionFile");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("SubmissionFileId");

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.ContentType)
                .HasMaxLength(100);

            entity.Property(e => e.UploadedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationship
            entity.HasOne(e => e.Submission)
                .WithMany(s => s.Files)
                .HasForeignKey(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Cấu hình cho OpsSubmissionRecipient
        modelBuilder.Entity<OpsSubmissionRecipient>(entity =>
        {
            entity.ToTable("OpsSubmissionRecipient");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("RecipientId");

            entity.Property(e => e.RecipientType)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("CC");

            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);

            entity.Property(e => e.ReadAt)
                .HasColumnType("datetime2(0)");

            // Relationships
            entity.HasOne(e => e.Submission)
                .WithMany(s => s.Recipients)
                .HasForeignKey(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.RecipientUser)
                .WithMany()
                .HasForeignKey(e => e.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);

            // Index
            entity.HasIndex(e => e.SubmissionId);
            entity.HasIndex(e => e.RecipientUserId);
        });

        // Cấu hình cho OpsApproval
        modelBuilder.Entity<OpsApproval>(entity =>
        {
            entity.ToTable("OpsApproval");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ApprovalId");

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Note)
                .HasMaxLength(500);

            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationships
            entity.HasOne(e => e.Submission)
                .WithMany() // No navigation back needed for now
                .HasForeignKey(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApproverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity fields
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => !e.IsDeleted);

            // Indexes
            entity.HasIndex(e => e.SubmissionId);
            entity.HasIndex(e => e.ApproverUserId);
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
