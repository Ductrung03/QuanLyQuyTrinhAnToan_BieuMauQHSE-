using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRecipientTableFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa bảng và tạo lại với cấu trúc đúng
            migrationBuilder.Sql(@"
                IF OBJECT_ID('OpsSubmissionRecipient', 'U') IS NOT NULL
                    DROP TABLE OpsSubmissionRecipient;
            ");

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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpsSubmissionRecipient", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_OpsSubmissionRecipient_OpsSubmission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "OpsSubmission",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpsSubmissionRecipient_AspNetUsers_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmissionRecipient_SubmissionId",
                table: "OpsSubmissionRecipient",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpsSubmissionRecipient_RecipientUserId",
                table: "OpsSubmissionRecipient",
                column: "RecipientUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpsSubmissionRecipient");
        }
    }
}
