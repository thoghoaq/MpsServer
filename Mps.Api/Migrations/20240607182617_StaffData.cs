using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class StaffData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Staff",
                newName: "IdentityCardFrontPath");

            migrationBuilder.RenameColumn(
                name: "AvatarPath",
                table: "Staff",
                newName: "IdentityCardBackPath");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Staff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificatePath",
                table: "Staff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityCard",
                table: "Staff",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "CertificatePath",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "IdentityCard",
                table: "Staff");

            migrationBuilder.RenameColumn(
                name: "IdentityCardFrontPath",
                table: "Staff",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "IdentityCardBackPath",
                table: "Staff",
                newName: "AvatarPath");
        }
    }
}
