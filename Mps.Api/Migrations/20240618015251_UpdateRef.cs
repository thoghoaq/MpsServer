using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentRef",
                table: "PaymentRef");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PaymentRef",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentRef",
                table: "PaymentRef",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRef_PaymentId",
                table: "PaymentRef",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentRef",
                table: "PaymentRef");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRef_PaymentId",
                table: "PaymentRef");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PaymentRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentRef",
                table: "PaymentRef",
                column: "PaymentId");
        }
    }
}
