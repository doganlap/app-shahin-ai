using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grc.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowDefinitionsWithLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "WorkflowDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "WorkflowDefinitions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "WorkflowDefinitions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "WorkflowDefinitions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WorkflowDefinitions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WorkflowDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
