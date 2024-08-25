using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShopImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessLicenseImage",
                table: "Shops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityBackImage",
                table: "ShopOwners",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityFrontImage",
                table: "ShopOwners",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "ShopOwners",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessLicenseImage",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "IdentityBackImage",
                table: "ShopOwners");

            migrationBuilder.DropColumn(
                name: "IdentityFrontImage",
                table: "ShopOwners");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "ShopOwners");
        }
    }
}
