using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace autocare_api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalNotes",
                table: "WorkshopProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "WorkshopProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "WorkshopProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByAdminId",
                table: "WorkshopProfiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopProfiles_ApprovalStatus",
                table: "WorkshopProfiles",
                column: "ApprovalStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkshopProfiles_ApprovalStatus",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "ApprovalNotes",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "WorkshopProfiles");

            migrationBuilder.DropColumn(
                name: "ReviewedByAdminId",
                table: "WorkshopProfiles");
        }
    }
}
