using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShopOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Suppliers_SupplierId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Suppliers_SupplierUserId",
                table: "Shops");

            migrationBuilder.RenameColumn(
                name: "SupplierUserId",
                table: "Shops",
                newName: "ShopOwnerUserId");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Shops",
                newName: "ShopOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_SupplierUserId",
                table: "Shops",
                newName: "IX_Shops_ShopOwnerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_SupplierId",
                table: "Shops",
                newName: "IX_Shops_ShopOwnerId");

            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Suppliers_ShopOwnerId",
                table: "Shops",
                column: "ShopOwnerId",
                principalTable: "Suppliers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Suppliers_ShopOwnerUserId",
                table: "Shops",
                column: "ShopOwnerUserId",
                principalTable: "Suppliers",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Suppliers_ShopOwnerId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Suppliers_ShopOwnerUserId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ShopOwnerUserId",
                table: "Shops",
                newName: "SupplierUserId");

            migrationBuilder.RenameColumn(
                name: "ShopOwnerId",
                table: "Shops",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_ShopOwnerUserId",
                table: "Shops",
                newName: "IX_Shops_SupplierUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_ShopOwnerId",
                table: "Shops",
                newName: "IX_Shops_SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Suppliers_SupplierId",
                table: "Shops",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Suppliers_SupplierUserId",
                table: "Shops",
                column: "SupplierUserId",
                principalTable: "Suppliers",
                principalColumn: "UserId");
        }
    }
}
