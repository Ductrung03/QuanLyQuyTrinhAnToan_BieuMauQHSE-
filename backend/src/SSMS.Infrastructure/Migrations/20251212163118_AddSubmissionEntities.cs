using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpsSubmission",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    RecipientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    RecipientUserId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_OpsSubmissionRecipient", x => x.RecipientId);
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
                });

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
                name: "IX_OpsSubmissionRecipient_SubmissionId",
                table: "OpsSubmissionRecipient",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpsSubmissionFile");

            migrationBuilder.DropTable(
                name: "OpsSubmissionRecipient");

            migrationBuilder.DropTable(
                name: "OpsSubmission");
        }
    }
}
