using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mps.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffCodeFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffCode",
                table: "Staff",
                type: "text",
                nullable: true);

            // Create sequence
            migrationBuilder.Sql(@"
                CREATE SEQUENCE staff_code_seq;
            ");

            // Create function
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION generate_staff_code()
                RETURNS text AS $$
                DECLARE
                    code text;
                BEGIN
                    -- Example code generation logic
                    code := 'SC' || to_char(nextval('staff_code_seq'), 'FM0000');
                    RETURN code;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Update existing records with a StaffCode
            migrationBuilder.Sql(@"
                UPDATE ""Staff""
                SET ""StaffCode"" = 'SC' || to_char(nextval('staff_code_seq'), 'FM0000')
                WHERE ""StaffCode"" IS NULL;
            ");

            // Alter table to set default value
            migrationBuilder.Sql(@"
                ALTER TABLE ""Staff""
                ALTER COLUMN ""StaffCode""
                SET DEFAULT generate_staff_code();
            ");

            // Alter table to set not null
            migrationBuilder.Sql(@"
                ALTER TABLE ""Staff""
                ALTER COLUMN ""StaffCode""
                SET NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop function
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS generate_staff_code();
            ");

            // Drop sequence
            migrationBuilder.Sql(@"
                DROP SEQUENCE IF EXISTS staff_code_seq;
            ");

            // Remove default value from column
            migrationBuilder.Sql(@"
                ALTER TABLE ""Staff""
                ALTER COLUMN ""StaffCode""
                DROP DEFAULT;
            ");

            migrationBuilder.DropColumn(
                name: "StaffCode",
                table: "Staff");
        }
    }
}
