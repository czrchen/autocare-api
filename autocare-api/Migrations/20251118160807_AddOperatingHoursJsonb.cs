using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    public partial class AddOperatingHoursJsonb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"WorkshopProfiles\" " +
                "SET \"OperatingHours\" = '{}' " +
                "WHERE \"OperatingHours\" IS NULL OR TRIM(\"OperatingHours\") = '';"
            );

            migrationBuilder.Sql(
                "ALTER TABLE \"WorkshopProfiles\" " +
                "ALTER COLUMN \"OperatingHours\" TYPE jsonb USING \"OperatingHours\"::jsonb;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"WorkshopProfiles\" " +
                "ALTER COLUMN \"OperatingHours\" TYPE text;"
            );
        }
    }
}
