using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _0001_InitialClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentUnitId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_Unit_Unit_ParentUnitId",
                        column: x => x.ParentUnitId,
                        principalTable: "Unit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "User"),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_AppUser_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUser_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpsAuditLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TargetId = table.Column<int>(type: "int", nullable: true),
                    TargetName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OldData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionTime = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsAuditLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_OpsAuditLog_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpsProcedure",
                columns: table => new
                {
                    ProcedureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OwnerUserId = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    State = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft"),
                    AuthorUserId = table.Column<int>(type: "int", nullable: true),
                    ApproverUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsProcedure", x => x.ProcedureId);
                    table.ForeignKey(
                        name: "FK_OpsProcedure_AppUser_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpsProcedure_AppUser_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpsProcedure_AppUser_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPermission",
                columns: table => new
                {
                    UserPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermission", x => x.UserPermissionId);
                    table.ForeignKey(
                        name: "FK_UserPermission_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpsProcedureDocument",
                columns: table => new
                {
                    ProcedureDocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcedureId = table.Column<int>(type: "int", nullable: false),
                    DocVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsProcedureDocument", x => x.ProcedureDocId);
                    table.ForeignKey(
                        name: "FK_OpsProcedureDocument_OpsProcedure_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "OpsProcedure",
                        principalColumn: "ProcedureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpsTemplate",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateKey = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ProcedureId = table.Column<int>(type: "int", nullable: false),
                    TemplateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Form"),
                    State = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsTemplate", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_OpsTemplate_OpsProcedure_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "OpsProcedure",
                        principalColumn: "ProcedureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpsSubmission",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcedureId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: true),
                    SubmittedByUserId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Submitted"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecalledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecallReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsSubmission", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_OpsSubmission_AppUser_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpsSubmission_OpsProcedure_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "OpsProcedure",
                        principalColumn: "ProcedureId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpsSubmission_OpsTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "OpsTemplate",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpsApproval",
                columns: table => new
                {
                    ApprovalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    ApproverUserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsApproval", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_OpsApproval_AppUser_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpsApproval_OpsSubmission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "OpsSubmission",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpsSubmissionFile",
                columns: table => new
                {
                    SubmissionFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsSubmissionFile", x => x.SubmissionFileId);
                    table.ForeignKey(
                        name: "FK_OpsSubmissionFile_OpsSubmission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "OpsSubmission",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpsSubmissionRecipient",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    RecipientUserId = table.Column<int>(type: "int", nullable: true),
                    RecipientRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecipientType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "CC"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsSubmissionRecipient", x => new { x.SubmissionId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_OpsSubmissionRecipient_AppUser_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpsSubmissionRecipient_OpsSubmission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "OpsSubmission",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpsSubmissionRecipient_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Unit",
                columns: new[] { "UnitId", "UnitCode", "CreatedAt", "CreatedBy", "Description", "IsActive", "UnitName", "ParentUnitId", "UnitType", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "HQ", new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5177), null, "Văn phòng trụ sở chính", true, "Trụ sở chính", null, "Headquarters", null, null });

            migrationBuilder.InsertData(
                table: "AppUser",
                columns: new[] { "UserId", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Phone", "Position", "Role", "RoleId", "UnitId", "UpdatedAt", "UpdatedBy", "UserName" },
                values: new object[] { 1, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5491), null, "admin@ssms.com", "Quản trị viên hệ thống", true, null, null, null, "System Administrator", "Admin", null, 1, null, null, "admin" });

            migrationBuilder.InsertData(
                table: "Unit",
                columns: new[] { "UnitId", "UnitCode", "CreatedAt", "CreatedBy", "Description", "IsActive", "UnitName", "ParentUnitId", "UnitType", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 2, "SHIP001", new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5180), null, "Tàu khai thác số 1", true, "Tàu Hải Phòng 01", 1, "Ship", null, null },
                    { 3, "SHIP002", new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5182), null, "Tàu khai thác số 2", true, "Tàu Hải Phòng 02", 1, "Ship", null, null },
                    { 4, "DEPT-QHSE", new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5184), null, "Phòng Quản lý Chất lượng, An toàn và Môi trường", true, "Phòng QHSE", 1, "Department", null, null },
                    { 5, "DEPT-OPS", new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5186), null, "Phòng Khai thác và Vận hành", true, "Phòng Khai thác", 1, "Department", null, null }
                });

            migrationBuilder.InsertData(
                table: "AppUser",
                columns: new[] { "UserId", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Phone", "Position", "Role", "RoleId", "UnitId", "UpdatedAt", "UpdatedBy", "UserName" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5494), null, "qhse.manager@ssms.com", "Nguyễn Văn An", true, null, null, "0901234567", "QHSE Manager", "Manager", null, 4, null, null, "qhse.manager" },
                    { 3, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5496), null, "ops.manager@ssms.com", "Trần Thị Bình", true, null, null, "0901234568", "Operations Manager", "Manager", null, 5, null, null, "ops.manager" },
                    { 4, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5498), null, "captain.ship001@ssms.com", "Lê Văn Cường", true, null, null, "0901234569", "Ship Captain", "User", null, 2, null, null, "ship001.captain" },
                    { 5, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5499), null, "officer.ship001@ssms.com", "Phạm Thị Dung", true, null, null, "0901234570", "Safety Officer", "User", null, 2, null, null, "ship001.officer" },
                    { 6, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5501), null, "captain.ship002@ssms.com", "Hoàng Văn Em", true, null, null, "0901234571", "Ship Captain", "User", null, 3, null, null, "ship002.captain" },
                    { 7, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5503), null, "officer.ship002@ssms.com", "Đỗ Thị Phương", true, null, null, "0901234572", "Safety Officer", "User", null, 3, null, null, "ship002.officer" },
                    { 8, new DateTime(2026, 1, 28, 8, 20, 48, 324, DateTimeKind.Utc).AddTicks(5505), null, "qhse.staff@ssms.com", "Vũ Văn Giang", true, null, null, "0901234573", "QHSE Staff", "User", null, 4, null, null, "qhse.staff" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_RoleId",
                table: "AppUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_UnitId",
                table: "AppUser",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsApproval_ApproverUserId",
                table: "OpsApproval",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsApproval_SubmissionId",
                table: "OpsApproval",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsAuditLog_Action",
                table: "OpsAuditLog",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_OpsAuditLog_ActionTime",
                table: "OpsAuditLog",
                column: "ActionTime");

            migrationBuilder.CreateIndex(
                name: "IX_OpsAuditLog_TargetType",
                table: "OpsAuditLog",
                column: "TargetType");

            migrationBuilder.CreateIndex(
                name: "IX_OpsAuditLog_UserId",
                table: "OpsAuditLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsProcedure_ApproverUserId",
                table: "OpsProcedure",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsProcedure_AuthorUserId",
                table: "OpsProcedure",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsProcedure_Code",
                table: "OpsProcedure",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpsProcedure_OwnerUserId",
                table: "OpsProcedure",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsProcedureDocument_ProcedureId",
                table: "OpsProcedureDocument",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmission_ProcedureId",
                table: "OpsSubmission",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmission_Status",
                table: "OpsSubmission",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmission_SubmittedByUserId",
                table: "OpsSubmission",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmission_TemplateId",
                table: "OpsSubmission",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmissionFile_SubmissionId",
                table: "OpsSubmissionFile",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmissionRecipient_RecipientUserId",
                table: "OpsSubmissionRecipient",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmissionRecipient_UnitId",
                table: "OpsSubmissionRecipient",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsTemplate_ProcedureId",
                table: "OpsTemplate",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Code",
                table: "Permission",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Module",
                table: "Permission",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Code",
                table: "Role",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_ParentUnitId",
                table: "Unit",
                column: "ParentUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_PermissionId",
                table: "UserPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_UserId_PermissionId",
                table: "UserPermission",
                columns: new[] { "UserId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpsApproval");

            migrationBuilder.DropTable(
                name: "OpsAuditLog");

            migrationBuilder.DropTable(
                name: "OpsProcedureDocument");

            migrationBuilder.DropTable(
                name: "OpsSubmissionFile");

            migrationBuilder.DropTable(
                name: "OpsSubmissionRecipient");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "UserPermission");

            migrationBuilder.DropTable(
                name: "OpsSubmission");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "OpsTemplate");

            migrationBuilder.DropTable(
                name: "OpsProcedure");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Unit");
        }
    }
}
