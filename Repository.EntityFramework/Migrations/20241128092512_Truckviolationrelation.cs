using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Truckviolationrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TruckViolationId",
                table: "TruckDetails",
                newName: "TruckViolationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckDetails_TruckViolationTypeId",
                table: "TruckDetails",
                column: "TruckViolationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TruckDetails_TruckViolationTypes_TruckViolationTypeId",
                table: "TruckDetails",
                column: "TruckViolationTypeId",
                principalTable: "TruckViolationTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TruckDetails_TruckViolationTypes_TruckViolationTypeId",
                table: "TruckDetails");

            migrationBuilder.DropIndex(
                name: "IX_TruckDetails_TruckViolationTypeId",
                table: "TruckDetails");

            migrationBuilder.RenameColumn(
                name: "TruckViolationTypeId",
                table: "TruckDetails",
                newName: "TruckViolationId");
        }
    }
}
