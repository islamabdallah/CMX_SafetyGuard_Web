using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class addViolationTypeAccuracyLaveltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViolationTypeAccuracyLavels",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    ViolationTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationTypeAccuracyLavels", x => new { x.LevelId, x.ViolationTypeId });
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
