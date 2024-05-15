using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDevice_Users_UserId",
                table: "UserDevice");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDevice_Users_UserId1",
                table: "UserDevice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDevice",
                table: "UserDevice");

            migrationBuilder.RenameTable(
                name: "UserDevice",
                newName: "UserDevices");

            migrationBuilder.RenameIndex(
                name: "IX_UserDevice_UserId1",
                table: "UserDevices",
                newName: "IX_UserDevices_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_UserDevice_UserId",
                table: "UserDevices",
                newName: "IX_UserDevices_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDevices",
                table: "UserDevices",
                column: "UserDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevices_Users_UserId",
                table: "UserDevices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevices_Users_UserId1",
                table: "UserDevices",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDevices_Users_UserId",
                table: "UserDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDevices_Users_UserId1",
                table: "UserDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDevices",
                table: "UserDevices");

            migrationBuilder.RenameTable(
                name: "UserDevices",
                newName: "UserDevice");

            migrationBuilder.RenameIndex(
                name: "IX_UserDevices_UserId1",
                table: "UserDevice",
                newName: "IX_UserDevice_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevice",
                newName: "IX_UserDevice_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDevice",
                table: "UserDevice",
                column: "UserDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevice_Users_UserId",
                table: "UserDevice",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevice_Users_UserId1",
                table: "UserDevice",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
