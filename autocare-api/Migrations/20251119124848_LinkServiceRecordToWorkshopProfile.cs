using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    /// <inheritdoc />
    public partial class LinkServiceRecordToWorkshopProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Users_WorkshopId",
                table: "ServiceRecords");

            migrationBuilder.RenameColumn(
                name: "WorkshopId",
                table: "ServiceRecords",
                newName: "WorkshopProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceRecords_WorkshopId",
                table: "ServiceRecords",
                newName: "IX_ServiceRecords_WorkshopProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_WorkshopProfiles_WorkshopProfileId",
                table: "ServiceRecords",
                column: "WorkshopProfileId",
                principalTable: "WorkshopProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_WorkshopProfiles_WorkshopProfileId",
                table: "ServiceRecords");

            migrationBuilder.RenameColumn(
                name: "WorkshopProfileId",
                table: "ServiceRecords",
                newName: "WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceRecords_WorkshopProfileId",
                table: "ServiceRecords",
                newName: "IX_ServiceRecords_WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Users_WorkshopId",
                table: "ServiceRecords",
                column: "WorkshopId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
