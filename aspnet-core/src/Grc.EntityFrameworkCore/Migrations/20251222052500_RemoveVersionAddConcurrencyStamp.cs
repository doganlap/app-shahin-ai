using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grc.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVersionAddConcurrencyStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop Version column if it exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Regulators' AND column_name = 'Version'
                    ) THEN
                        ALTER TABLE ""Regulators"" DROP COLUMN ""Version"";
                    END IF;
                END $$;
            ");

            // Add ConcurrencyStamp if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Regulators' AND column_name = 'ConcurrencyStamp'
                    ) THEN
                        ALTER TABLE ""Regulators"" ADD COLUMN ""ConcurrencyStamp"" text NULL;
                    END IF;
                END $$;
            ");

            // Add CreatorId, LastModifierId, DeleterId, DeletionTime if they don't exist
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Regulators' AND column_name = 'CreatorId') THEN
                        ALTER TABLE ""Regulators"" ADD COLUMN ""CreatorId"" uuid NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Regulators' AND column_name = 'LastModifierId') THEN
                        ALTER TABLE ""Regulators"" ADD COLUMN ""LastModifierId"" uuid NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Regulators' AND column_name = 'DeleterId') THEN
                        ALTER TABLE ""Regulators"" ADD COLUMN ""DeleterId"" uuid NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Regulators' AND column_name = 'DeletionTime') THEN
                        ALTER TABLE ""Regulators"" ADD COLUMN ""DeletionTime"" timestamp with time zone NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Regulators' AND column_name = 'LastModificationTime') THEN
                        ALTER TABLE ""Regulators"" ADD COLUMN ""LastModificationTime"" timestamp with time zone NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add Version column back
            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Regulators",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Remove ConcurrencyStamp
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Regulators");

            // Remove audit fields
            migrationBuilder.DropColumn(name: "CreatorId", table: "Regulators");
            migrationBuilder.DropColumn(name: "LastModifierId", table: "Regulators");
            migrationBuilder.DropColumn(name: "DeleterId", table: "Regulators");
            migrationBuilder.DropColumn(name: "DeletionTime", table: "Regulators");
            migrationBuilder.DropColumn(name: "LastModificationTime", table: "Regulators");
        }
    }
}

