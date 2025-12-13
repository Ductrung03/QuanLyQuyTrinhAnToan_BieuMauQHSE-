using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingEntityColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns to OpsProcedureDocument
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'ContentType')
                    ALTER TABLE OpsProcedureDocument ADD ContentType nvarchar(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'FileSize')
                    ALTER TABLE OpsProcedureDocument ADD FileSize bigint NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
