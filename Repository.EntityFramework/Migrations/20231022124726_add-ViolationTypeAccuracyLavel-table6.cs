using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class addViolationTypeAccuracyLaveltable6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViolationTypeAccuracyLavels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    ViolationTypeId = table.Column<int>(type: "int", nullable: false),
                    LowestPercent = table.Column<float>(type: "real", nullable: false),
                    HighestPercent = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationTypeAccuracyLavels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationTypeAccuracyLavels_ViolationTypes_ViolationTypeId",
                        column: x => x.ViolationTypeId,
                        principalTable: "ViolationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViolationTypeAccuracyLavels_ViolationTypeId",
                table: "ViolationTypeAccuracyLavels",
                column: "ViolationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViolationTypeAccuracyLavels");
        }
    }
}
