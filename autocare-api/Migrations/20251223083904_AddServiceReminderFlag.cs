using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceReminderFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "ServiceRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "ServiceRecords");
        }
    }
}
