using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns to OpsProcedure
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'IsDeleted')
                    ALTER TABLE OpsProcedure ADD IsDeleted bit NOT NULL DEFAULT 0;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'CreatedAt')
                    ALTER TABLE OpsProcedure ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME();
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'UpdatedAt')
                    ALTER TABLE OpsProcedure ADD UpdatedAt datetime2 NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'CreatedBy')
                    ALTER TABLE OpsProcedure ADD CreatedBy nvarchar(max) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedure') AND name = 'UpdatedBy')
                    ALTER TABLE OpsProcedure ADD UpdatedBy nvarchar(max) NULL;
            ");

            // Add missing columns to OpsProcedureDocument
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'IsDeleted')
                    ALTER TABLE OpsProcedureDocument ADD IsDeleted bit NOT NULL DEFAULT 0;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'CreatedAt')
                    ALTER TABLE OpsProcedureDocument ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME();
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'UpdatedAt')
                    ALTER TABLE OpsProcedureDocument ADD UpdatedAt datetime2 NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'CreatedBy')
                    ALTER TABLE OpsProcedureDocument ADD CreatedBy nvarchar(max) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsProcedureDocument') AND name = 'UpdatedBy')
                    ALTER TABLE OpsProcedureDocument ADD UpdatedBy nvarchar(max) NULL;
            ");

            // Add missing columns to OpsTemplate
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'IsDeleted')
                    ALTER TABLE OpsTemplate ADD IsDeleted bit NOT NULL DEFAULT 0;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'CreatedAt')
                    ALTER TABLE OpsTemplate ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME();
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'UpdatedAt')
                    ALTER TABLE OpsTemplate ADD UpdatedAt datetime2 NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'CreatedBy')
                    ALTER TABLE OpsTemplate ADD CreatedBy nvarchar(max) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsTemplate') AND name = 'UpdatedBy')
                    ALTER TABLE OpsTemplate ADD UpdatedBy nvarchar(max) NULL;
            ");

            // Add missing columns to OpsSubmission
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'IsDeleted')
                    ALTER TABLE OpsSubmission ADD IsDeleted bit NOT NULL DEFAULT 0;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'CreatedAt')
                    ALTER TABLE OpsSubmission ADD CreatedAt datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME();
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'UpdatedAt')
                    ALTER TABLE OpsSubmission ADD UpdatedAt datetime2 NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'CreatedBy')
                    ALTER TABLE OpsSubmission ADD CreatedBy nvarchar(max) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('OpsSubmission') AND name = 'UpdatedBy')
                    ALTER TABLE OpsSubmission ADD UpdatedBy nvarchar(max) NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
