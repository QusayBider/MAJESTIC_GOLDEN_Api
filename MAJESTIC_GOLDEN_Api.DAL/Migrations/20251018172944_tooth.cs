using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAJESTIC_GOLDEN_Api.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tooth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PatientTeeth");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth",
                columns: new[] { "ToothId", "PatientId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PatientTeeth",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth",
                column: "Id");
        }
    }
}
