using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class IsFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeedbacked",
                table: "OrderDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeedbacked",
                table: "OrderDetails");
        }
    }
}
