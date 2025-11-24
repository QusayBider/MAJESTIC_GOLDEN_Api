using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAJESTIC_GOLDEN_Api.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus_Ar",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus_En",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation_Ar",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation_En",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentPlan_Ar",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentPlan_En",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaritalStatus_Ar",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MaritalStatus_En",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Occupation_Ar",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Occupation_En",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "TreatmentPlan_Ar",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "TreatmentPlan_En",
                table: "Patients");
        }
    }
}
