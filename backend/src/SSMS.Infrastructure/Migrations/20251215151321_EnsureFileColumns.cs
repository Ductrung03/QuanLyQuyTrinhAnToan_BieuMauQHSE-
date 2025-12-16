using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureFileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột ContentType nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsTemplate]') AND name = 'ContentType')
                BEGIN
                    ALTER TABLE [OpsTemplate] ADD [ContentType] NVARCHAR(100) NULL;
                END
            ");

            // Thêm cột FileSize nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsTemplate]') AND name = 'FileSize')
                BEGIN
                    ALTER TABLE [OpsTemplate] ADD [FileSize] BIGINT NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "OpsTemplate");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "OpsTemplate");
        }
    }
}
