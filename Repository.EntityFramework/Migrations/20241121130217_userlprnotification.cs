using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class userlprnotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViolationNotification_Violations_ViolationID",
                table: "ViolationNotification");

            migrationBuilder.DropColumn(
                name: "LPR_Id",
                table: "ViolationNotification");

            migrationBuilder.AlterColumn<long>(
                name: "ViolationID",
                table: "ViolationNotification",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ViolationNotification_Violations_ViolationID",
                table: "ViolationNotification",
                column: "ViolationID",
                principalTable: "Violations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViolationNotification_Violations_ViolationID",
                table: "ViolationNotification");

            migrationBuilder.AlterColumn<long>(
                name: "ViolationID",
                table: "ViolationNotification",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "LPR_Id",
                table: "ViolationNotification",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ViolationNotification_Violations_ViolationID",
                table: "ViolationNotification",
                column: "ViolationID",
                principalTable: "Violations",
                principalColumn: "Id");
        }
    }
}
