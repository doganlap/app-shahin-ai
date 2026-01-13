using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrcMvc.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCrossDatabaseForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints that reference ApplicationUser table
            // ApplicationUser is in GrcAuthDb, but TenantUsers and PlatformAdmins are in GrcMvcDb
            // Cross-database foreign keys are not supported in PostgreSQL
            
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_TenantUsers_ApplicationUser_UserId'
                        AND table_name = 'TenantUsers'
                    ) THEN
                        ALTER TABLE ""TenantUsers"" DROP CONSTRAINT ""FK_TenantUsers_ApplicationUser_UserId"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_TenantUsers_AspNetUsers_UserId'
                        AND table_name = 'TenantUsers'
                    ) THEN
                        ALTER TABLE ""TenantUsers"" DROP CONSTRAINT ""FK_TenantUsers_AspNetUsers_UserId"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_PlatformAdmins_ApplicationUser_UserId'
                        AND table_name = 'PlatformAdmins'
                    ) THEN
                        ALTER TABLE ""PlatformAdmins"" DROP CONSTRAINT ""FK_PlatformAdmins_ApplicationUser_UserId"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_PlatformAdmins_AspNetUsers_UserId'
                        AND table_name = 'PlatformAdmins'
                    ) THEN
                        ALTER TABLE ""PlatformAdmins"" DROP CONSTRAINT ""FK_PlatformAdmins_AspNetUsers_UserId"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Cannot recreate cross-database foreign keys
            // Application-level validation will ensure referential integrity
        }
    }
}
