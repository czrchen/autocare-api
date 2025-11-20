using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceIdToServiceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOcrUsed",
                table: "ServiceRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "ServiceRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_ServiceId",
                table: "ServiceRecords",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Services_ServiceId",
                table: "ServiceRecords",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Services_ServiceId",
                table: "ServiceRecords");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRecords_ServiceId",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceRecords");

            migrationBuilder.AddColumn<bool>(
                name: "IsOcrUsed",
                table: "ServiceRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
