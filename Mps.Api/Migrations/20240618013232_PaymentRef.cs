using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class PaymentRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "Payments");

            migrationBuilder.CreateTable(
                name: "PaymentRef",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "integer", nullable: false),
                    RefId = table.Column<int>(type: "integer", nullable: true),
                    MerchantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRef", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_PaymentRef_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentRef");

            migrationBuilder.AddColumn<int>(
                name: "MerchantId",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefId",
                table: "Payments",
                type: "integer",
                nullable: true);
        }
    }
}
