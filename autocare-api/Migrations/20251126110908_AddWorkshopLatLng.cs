using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopLatLng : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "WorkshopProfiles",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "WorkshopProfiles",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "WorkshopProfiles");
        }
    }
}
