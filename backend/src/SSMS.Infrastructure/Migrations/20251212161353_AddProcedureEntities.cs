using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_OpsTemplate_ProcedureId",
                table: "OpsTemplate",
                column: "ProcedureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpsProcedureDocument");

            migrationBuilder.DropTable(
                name: "OpsTemplate");

            migrationBuilder.DropTable(
                name: "OpsProcedure");
        }
    }
}
