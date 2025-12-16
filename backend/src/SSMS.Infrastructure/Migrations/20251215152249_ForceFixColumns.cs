using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ForceFixColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                BEGIN TRY
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsTemplate]') AND name = 'ContentType')
                    BEGIN
                        ALTER TABLE [OpsTemplate] ADD [ContentType] NVARCHAR(100) NULL
                        PRINT 'Added ContentType column'
                    END
                END TRY
                BEGIN CATCH
                    PRINT 'Error adding ContentType: ' + ERROR_MESSAGE()
                END CATCH
                
                BEGIN TRY
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsTemplate]') AND name = 'FileSize')
                    BEGIN
                        ALTER TABLE [OpsTemplate] ADD [FileSize] BIGINT NULL
                        PRINT 'Added FileSize column'
                    END
                END TRY
                BEGIN CATCH
                    PRINT 'Error adding FileSize: ' + ERROR_MESSAGE()
                END CATCH
                
                BEGIN TRY
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OpsAuditLog]') AND name = 'IsDeleted')
                    BEGIN
                        ALTER TABLE [OpsAuditLog] ADD [IsDeleted] BIT NOT NULL DEFAULT 0
                        PRINT 'Added IsDeleted column to OpsAuditLog'
                    END
                END TRY
                BEGIN CATCH
                    PRINT 'Error adding IsDeleted: ' + ERROR_MESSAGE()
                END CATCH
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
