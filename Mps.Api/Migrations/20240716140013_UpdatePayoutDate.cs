using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayoutDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Payouts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PayoutDate",
                table: "Payouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Payouts");

            migrationBuilder.DropColumn(
                name: "PayoutDate",
                table: "Payouts");
        }
    }
}
