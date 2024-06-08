using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId",
                table: "PaymentSignatures");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId1",
                table: "PaymentSignatures");

            migrationBuilder.DropIndex(
                name: "IX_PaymentSignatures_PaymentId1",
                table: "PaymentSignatures");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "PaymentSignatures");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserDeviceId",
                table: "UserDevices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ShopId",
                table: "Shops",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductImageId",
                table: "ProductImages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "ProductCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "ProductCategories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BrandName",
                table: "ProductBrands",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "ProductBrands",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PaymentStatusName",
                table: "PaymentStatuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PaymentStatusId",
                table: "PaymentStatuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PaymentSignatureId",
                table: "PaymentSignatures",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PaymentRefId",
                table: "Payments",
                newName: "RefId");

            migrationBuilder.RenameColumn(
                name: "PaymentLanguage",
                table: "Payments",
                newName: "Language");

            migrationBuilder.RenameColumn(
                name: "PaymentCurrency",
                table: "Payments",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "PaymentContent",
                table: "Payments",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Payments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodName",
                table: "PaymentMethods",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "PaymentMethods",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderStatusName",
                table: "OrderStatuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OrderStatusId",
                table: "OrderStatuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Orders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderProgressName",
                table: "OrderProgress",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OrderProgressId",
                table: "OrderProgress",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "OrderDetails",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Customers",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentSignatures",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "PaymentSignatureId",
                table: "Payments",
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

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentSignatureId2",
                table: "Payments",
                column: "PaymentSignatureId2");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProgress_OrderId1",
                table: "OrderProgress",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_Id",
                table: "Customers",
                column: "Id",
                principalTable: "Users",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProgress_Orders_OrderId1",
                table: "OrderProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentSignatures_PaymentSignatureId2",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId",
                table: "PaymentSignatures");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentSignatureId2",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_OrderProgress_OrderId1",
                table: "OrderProgress");

            migrationBuilder.DropColumn(
                name: "PaymentSignatureId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentSignatureId2",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderProgress");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserDevices",
                newName: "UserDeviceId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Shops",
                newName: "ShopId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductImages",
                newName: "ProductImageId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductCategories",
                newName: "CategoryName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductCategories",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductBrands",
                newName: "BrandName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductBrands",
                newName: "BrandId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PaymentStatuses",
                newName: "PaymentStatusName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PaymentStatuses",
                newName: "PaymentStatusId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PaymentSignatures",
                newName: "PaymentSignatureId");

            migrationBuilder.RenameColumn(
                name: "RefId",
                table: "Payments",
                newName: "PaymentRefId");

            migrationBuilder.RenameColumn(
                name: "Language",
                table: "Payments",
                newName: "PaymentLanguage");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Payments",
                newName: "PaymentCurrency");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Payments",
                newName: "PaymentContent");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Payments",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PaymentMethods",
                newName: "PaymentMethodName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PaymentMethods",
                newName: "PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderStatuses",
                newName: "OrderStatusName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderStatuses",
                newName: "OrderStatusId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Orders",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderProgress",
                newName: "OrderProgressName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderProgress",
                newName: "OrderProgressId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderDetails",
                newName: "OrderDetailId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentSignatures",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId1",
                table: "PaymentSignatures",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSignatures_PaymentId1",
                table: "PaymentSignatures",
                column: "PaymentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId",
                table: "PaymentSignatures",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentSignatures_Payments_PaymentId1",
                table: "PaymentSignatures",
                column: "PaymentId1",
                principalTable: "Payments",
                principalColumn: "PaymentId");
        }
    }
}
