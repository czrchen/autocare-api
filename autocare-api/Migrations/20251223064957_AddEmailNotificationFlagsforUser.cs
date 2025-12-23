using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailNotificationFlagsforUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailNotificationsConfirmed",
                table: "WorkshopProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotificationsRequested",
                table: "WorkshopProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotificationsConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotificationsRequested",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailNotificationsConfirmed",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "EmailNotificationsRequested",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "EmailNotificationsConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailNotificationsRequested",
                table: "Users");
        }
    }
}
