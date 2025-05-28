using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class accurate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Accurate",
                table: "Violations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Observation",
                table: "Violations",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accurate",
                table: "Violations");

            migrationBuilder.DropColumn(
                name: "Observation",
                table: "Violations");
        }
    }
}
