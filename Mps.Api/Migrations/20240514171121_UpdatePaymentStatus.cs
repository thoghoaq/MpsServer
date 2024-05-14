using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Suppliers_ShopOwnerId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Suppliers_ShopOwnerUserId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Users_UserId",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.DeleteData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 5);

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "ShopOwners");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatusId",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopOwners",
                table: "ShopOwners",
                column: "UserId");

            migrationBuilder.UpdateData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 2,
                column: "PaymentStatusName",
                value: "Success");

            migrationBuilder.UpdateData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 3,
                column: "PaymentStatusName",
                value: "Failed");

            migrationBuilder.UpdateData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 4,
                column: "PaymentStatusName",
                value: "Expired");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentStatusId",
                table: "Payments",
                column: "PaymentStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentStatuses_PaymentStatusId",
                table: "Payments",
                column: "PaymentStatusId",
                principalTable: "PaymentStatuses",
                principalColumn: "PaymentStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOwners_Users_UserId",
                table: "ShopOwners",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_ShopOwners_ShopOwnerId",
                table: "Shops",
                column: "ShopOwnerId",
                principalTable: "ShopOwners",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_ShopOwners_ShopOwnerUserId",
                table: "Shops",
                column: "ShopOwnerUserId",
                principalTable: "ShopOwners",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentStatuses_PaymentStatusId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOwners_Users_UserId",
                table: "ShopOwners");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_ShopOwners_ShopOwnerId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_ShopOwners_ShopOwnerUserId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentStatusId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopOwners",
                table: "ShopOwners");

            migrationBuilder.DropColumn(
                name: "PaymentStatusId",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "ShopOwners",
                newName: "Suppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "UserId");

            migrationBuilder.UpdateData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 2,
                column: "PaymentStatusName",
                value: "Processing");

            migrationBuilder.UpdateData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 3,
                column: "PaymentStatusName",
                value: "Paid");

            migrationBuilder.UpdateData(
                table: "PaymentStatuses",
                keyColumn: "PaymentStatusId",
                keyValue: 4,
                column: "PaymentStatusName",
                value: "Cancelled");

            migrationBuilder.InsertData(
                table: "PaymentStatuses",
                columns: new[] { "PaymentStatusId", "PaymentStatusName" },
                values: new object[] { 5, "Refunded" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Users_UserId",
                table: "Suppliers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
