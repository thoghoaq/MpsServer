using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_ShopOwners_ShopOwnerUserId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_ShopOwnerUserId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "ShopOwnerUserId",
                table: "Shops");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopOwnerUserId",
                table: "Shops",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_ShopOwnerUserId",
                table: "Shops",
                column: "ShopOwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_ShopOwners_ShopOwnerUserId",
                table: "Shops",
                column: "ShopOwnerUserId",
                principalTable: "ShopOwners",
                principalColumn: "UserId");
        }
    }
}
