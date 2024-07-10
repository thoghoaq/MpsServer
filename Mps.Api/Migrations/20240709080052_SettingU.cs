using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class SettingU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopSetting_Shops_ShopId",
                table: "ShopSetting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopSetting",
                table: "ShopSetting");

            migrationBuilder.RenameTable(
                name: "ShopSetting",
                newName: "ShopSettings");

            migrationBuilder.RenameIndex(
                name: "IX_ShopSetting_ShopId",
                table: "ShopSettings",
                newName: "IX_ShopSettings_ShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopSettings",
                table: "ShopSettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopSettings_Shops_ShopId",
                table: "ShopSettings",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopSettings_Shops_ShopId",
                table: "ShopSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopSettings",
                table: "ShopSettings");

            migrationBuilder.RenameTable(
                name: "ShopSettings",
                newName: "ShopSetting");

            migrationBuilder.RenameIndex(
                name: "IX_ShopSettings_ShopId",
                table: "ShopSetting",
                newName: "IX_ShopSetting_ShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopSetting",
                table: "ShopSetting",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopSetting_Shops_ShopId",
                table: "ShopSetting",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
