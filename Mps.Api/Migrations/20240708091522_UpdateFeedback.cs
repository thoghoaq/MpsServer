using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ProductFeedbacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ProductFeedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeedbacks_UserId",
                table: "ProductFeedbacks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeedbacks_Customers_UserId",
                table: "ProductFeedbacks",
                column: "UserId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeedbacks_Customers_UserId",
                table: "ProductFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ProductFeedbacks_UserId",
                table: "ProductFeedbacks");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductFeedbacks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductFeedbacks");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Customers");
        }
    }
}
