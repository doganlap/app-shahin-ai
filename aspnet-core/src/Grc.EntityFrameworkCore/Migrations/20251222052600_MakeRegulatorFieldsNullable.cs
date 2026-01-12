using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grc.Migrations
{
    /// <inheritdoc />
    public partial class MakeRegulatorFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make LogoUrl, Website, ContactEmail, ContactPhone, ContactAddress nullable
            migrationBuilder.Sql(@"
                ALTER TABLE ""Regulators"" ALTER COLUMN ""LogoUrl"" DROP NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""Website"" DROP NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""ContactEmail"" DROP NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""ContactPhone"" DROP NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""ContactAddress"" DROP NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""JurisdictionEn"" DROP NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""JurisdictionAr"" DROP NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Make fields NOT NULL again (only if safe)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Regulators"" ALTER COLUMN ""LogoUrl"" SET NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""Website"" SET NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""ContactEmail"" SET NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""ContactPhone"" SET NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""ContactAddress"" SET NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""JurisdictionEn"" SET NOT NULL;
                ALTER TABLE ""Regulators"" ALTER COLUMN ""JurisdictionAr"" SET NOT NULL;
            ");
        }
    }
}

