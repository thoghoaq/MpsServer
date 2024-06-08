using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId1",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProgress_Orders_OrderId1",
                table: "OrderProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentSignatures_PaymentSignatureId2",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId",
                table: "PaymentSignatures");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId1",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDevices_Users_UserId1",
                table: "UserDevices");

            migrationBuilder.DropIndex(
                name: "IX_UserDevices_UserId1",
                table: "UserDevices");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId1",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_PaymentSignatures_PaymentId",
                table: "PaymentSignatures");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentSignatureId2",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_OrderProgress_OrderId1",
                table: "OrderProgress");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "PaymentSignatures");

            migrationBuilder.DropColumn(
                name: "PaymentSignatureId2",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderProgress");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentSignatureId",
                table: "Payments",
                column: "PaymentSignatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentSignatures_PaymentSignatureId",
                table: "Payments",
                column: "PaymentSignatureId",
                principalTable: "PaymentSignatures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentSignatures_PaymentSignatureId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentSignatureId",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserDevices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ProductImages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "PaymentSignatures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentSignatureId2",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "OrderProgress",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "OrderDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId1",
                table: "UserDevices",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId1",
                table: "ProductImages",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSignatures_PaymentId",
                table: "PaymentSignatures",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentSignatureId2",
                table: "Payments",
                column: "PaymentSignatureId2");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProgress_OrderId1",
                table: "OrderProgress",
                column: "OrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId1",
                table: "OrderDetails",
                column: "OrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId1",
                table: "OrderDetails",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProgress_Orders_OrderId1",
                table: "OrderProgress",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentSignatures_PaymentSignatureId2",
                table: "Payments",
                column: "PaymentSignatureId2",
                principalTable: "PaymentSignatures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId",
                table: "PaymentSignatures",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId1",
                table: "ProductImages",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDevices_Users_UserId1",
                table: "UserDevices",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
