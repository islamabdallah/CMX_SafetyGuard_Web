using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class removeViolationTypeAccuracyLaveltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {



 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViolationTypeAccuracyLavelId",
                table: "Violations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ViolationTypeAccuracyLavelId1",
                table: "Violations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ViolationTypeAccuracyLavel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ViolationTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HighestPercent = table.Column<float>(type: "real", nullable: false),
                    IsDelted = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    LowestPercent = table.Column<float>(type: "real", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationTypeAccuracyLavel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationTypeAccuracyLavel_ViolationTypes_ViolationTypeId",
                        column: x => x.ViolationTypeId,
                        principalTable: "ViolationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Violations_ViolationTypeAccuracyLavelId1",
                table: "Violations",
                column: "ViolationTypeAccuracyLavelId1");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationTypeAccuracyLavel_ViolationTypeId",
                table: "ViolationTypeAccuracyLavel",
                column: "ViolationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Violations_ViolationTypeAccuracyLavel_ViolationTypeAccuracyLavelId1",
                table: "Violations",
                column: "ViolationTypeAccuracyLavelId1",
                principalTable: "ViolationTypeAccuracyLavel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
