using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAJESTIC_GOLDEN_Api.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Laboratory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LabName",
                table: "LabRequests");

            migrationBuilder.AddColumn<int>(
                name: "LaboratoryId",
                table: "LabRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Laboratory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_LaboratoryId",
                table: "LabRequests",
                column: "LaboratoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabRequests_Laboratory_LaboratoryId",
                table: "LabRequests",
                column: "LaboratoryId",
                principalTable: "Laboratory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabRequests_Laboratory_LaboratoryId",
                table: "LabRequests");

            migrationBuilder.DropTable(
                name: "Laboratory");

            migrationBuilder.DropIndex(
                name: "IX_LabRequests_LaboratoryId",
                table: "LabRequests");

            migrationBuilder.DropColumn(
                name: "LaboratoryId",
                table: "LabRequests");

            migrationBuilder.AddColumn<string>(
                name: "LabName",
                table: "LabRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
