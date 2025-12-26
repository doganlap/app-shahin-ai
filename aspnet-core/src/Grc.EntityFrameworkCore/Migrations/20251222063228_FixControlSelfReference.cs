using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grc.Migrations
{
    /// <inheritdoc />
    public partial class FixControlSelfReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Controls_Controls_ControlId",
                table: "Controls");

            migrationBuilder.DropIndex(
                name: "IX_Controls_ControlId",
                table: "Controls");

            migrationBuilder.DropColumn(
                name: "ControlId",
                table: "Controls");

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ParentControlId",
                table: "Controls",
                column: "ParentControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Controls_Controls_ParentControlId",
                table: "Controls",
                column: "ParentControlId",
                principalTable: "Controls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Controls_Controls_ParentControlId",
                table: "Controls");

            migrationBuilder.DropIndex(
                name: "IX_Controls_ParentControlId",
                table: "Controls");

            migrationBuilder.AddColumn<Guid>(
                name: "ControlId",
                table: "Controls",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ControlId",
                table: "Controls",
                column: "ControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Controls_Controls_ControlId",
                table: "Controls",
                column: "ControlId",
                principalTable: "Controls",
                principalColumn: "Id");
        }
    }
}
