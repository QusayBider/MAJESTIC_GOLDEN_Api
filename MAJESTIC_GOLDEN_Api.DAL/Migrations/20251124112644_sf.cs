using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAJESTIC_GOLDEN_Api.DAL.Migrations
{
    /// <inheritdoc />
    public partial class sf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabRequests_Laboratory_LaboratoryId",
                table: "LabRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Laboratory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Laboratory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Laboratory",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratory_UserId",
                table: "Laboratory",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Laboratory_Users_UserId",
                table: "Laboratory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRequests_Laboratory_LaboratoryId",
                table: "LabRequests",
                column: "LaboratoryId",
                principalTable: "Laboratory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Laboratory_Users_UserId",
                table: "Laboratory");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRequests_Laboratory_LaboratoryId",
                table: "LabRequests");

            migrationBuilder.DropIndex(
                name: "IX_Laboratory_UserId",
                table: "Laboratory");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Laboratory");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Laboratory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Laboratory");

            migrationBuilder.AddForeignKey(
                name: "FK_LabRequests_Laboratory_LaboratoryId",
                table: "LabRequests",
                column: "LaboratoryId",
                principalTable: "Laboratory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
