using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    public partial class InitWorkshopAddressJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert existing column from TEXT → JSONB safely
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkshopProfiles""
                ALTER COLUMN ""Address""
                TYPE jsonb
                USING ""Address""::jsonb;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert JSONB back → TEXT (safe)
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkshopProfiles""
                ALTER COLUMN ""Address""
                TYPE text
                USING ""Address""::text;
            ");
        }
    }
}
