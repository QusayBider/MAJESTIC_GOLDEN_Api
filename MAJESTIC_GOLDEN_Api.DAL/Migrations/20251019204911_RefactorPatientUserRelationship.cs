using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAJESTIC_GOLDEN_Api.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPatientUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseTransfers_Patients_PatientId",
                table: "CaseTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Patients_PatientId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRequests_Patients_PatientId",
                table: "LabRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAttachments_Patients_PatientId",
                table: "PatientAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDebts_Patients_PatientId",
                table: "PatientDebts");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Branches_BranchId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientTeeth_Patients_PatientId",
                table: "PatientTeeth");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentCases_Patients_PatientId",
                table: "TreatmentCases");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentCases_PatientId",
                table: "TreatmentCases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth");

            migrationBuilder.DropIndex(
                name: "IX_PatientTeeth_PatientId",
                table: "PatientTeeth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_BranchId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_Email",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_Phone",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientDebts_PatientId",
                table: "PatientDebts");

            migrationBuilder.DropIndex(
                name: "IX_PatientAttachments_PatientId",
                table: "PatientAttachments");

            migrationBuilder.DropIndex(
                name: "IX_LabRequests_PatientId",
                table: "LabRequests");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PatientId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_CaseTransfers_PatientId",
                table: "CaseTransfers");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "TreatmentCases");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "PatientTeeth");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FullName_Ar",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FullName_En",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "PatientDebts");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "PatientAttachments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "LabRequests");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "CaseTransfers");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Occupation_En",
                table: "Patients",
                newName: "SpecialNotes_En");

            migrationBuilder.RenameColumn(
                name: "Occupation_Ar",
                table: "Patients",
                newName: "SpecialNotes_Ar");

            migrationBuilder.RenameColumn(
                name: "MaritalStatus_En",
                table: "Patients",
                newName: "InsuranceProvider");

            migrationBuilder.RenameColumn(
                name: "MaritalStatus_Ar",
                table: "Patients",
                newName: "InsurancePolicyNumber");

            migrationBuilder.RenameColumn(
                name: "Address_En",
                table: "Patients",
                newName: "EmergencyContactRelation");

            migrationBuilder.RenameColumn(
                name: "Address_Ar",
                table: "Patients",
                newName: "EmergencyContactPhone");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Ar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_En",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName_Ar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName_En",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation_Ar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation_En",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "TreatmentCases",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "PatientTeeth",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "PatientDebts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "PatientAttachments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "LabRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "Invoices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "CaseTransfers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientUserId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth",
                columns: new[] { "ToothId", "PatientUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_PatientUserId",
                table: "TreatmentCases",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTeeth_PatientUserId",
                table: "PatientTeeth",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDebts_PatientUserId",
                table: "PatientDebts",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAttachments_PatientUserId",
                table: "PatientAttachments",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_PatientUserId",
                table: "LabRequests",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientUserId",
                table: "Invoices",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTransfers_PatientUserId",
                table: "CaseTransfers",
                column: "PatientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientUserId",
                table: "Appointments",
                column: "PatientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientUserId",
                table: "Appointments",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseTransfers_Patients_PatientUserId",
                table: "CaseTransfers",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Patients_PatientUserId",
                table: "Invoices",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRequests_Patients_PatientUserId",
                table: "LabRequests",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAttachments_Patients_PatientUserId",
                table: "PatientAttachments",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDebts_Patients_PatientUserId",
                table: "PatientDebts",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTeeth_Patients_PatientUserId",
                table: "PatientTeeth",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentCases_Patients_PatientUserId",
                table: "TreatmentCases",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientUserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseTransfers_Patients_PatientUserId",
                table: "CaseTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Patients_PatientUserId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_LabRequests_Patients_PatientUserId",
                table: "LabRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAttachments_Patients_PatientUserId",
                table: "PatientAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientDebts_Patients_PatientUserId",
                table: "PatientDebts");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientTeeth_Patients_PatientUserId",
                table: "PatientTeeth");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentCases_Patients_PatientUserId",
                table: "TreatmentCases");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentCases_PatientUserId",
                table: "TreatmentCases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth");

            migrationBuilder.DropIndex(
                name: "IX_PatientTeeth_PatientUserId",
                table: "PatientTeeth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientDebts_PatientUserId",
                table: "PatientDebts");

            migrationBuilder.DropIndex(
                name: "IX_PatientAttachments_PatientUserId",
                table: "PatientAttachments");

            migrationBuilder.DropIndex(
                name: "IX_LabRequests_PatientUserId",
                table: "LabRequests");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PatientUserId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_CaseTransfers_PatientUserId",
                table: "CaseTransfers");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Address_Ar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address_En",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName_Ar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName_En",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Occupation_Ar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Occupation_En",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "TreatmentCases");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "PatientTeeth");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "PatientDebts");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "PatientAttachments");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "LabRequests");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "CaseTransfers");

            migrationBuilder.DropColumn(
                name: "PatientUserId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "SpecialNotes_En",
                table: "Patients",
                newName: "Occupation_En");

            migrationBuilder.RenameColumn(
                name: "SpecialNotes_Ar",
                table: "Patients",
                newName: "Occupation_Ar");

            migrationBuilder.RenameColumn(
                name: "InsuranceProvider",
                table: "Patients",
                newName: "MaritalStatus_En");

            migrationBuilder.RenameColumn(
                name: "InsurancePolicyNumber",
                table: "Patients",
                newName: "MaritalStatus_Ar");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactRelation",
                table: "Patients",
                newName: "Address_En");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactPhone",
                table: "Patients",
                newName: "Address_Ar");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "TreatmentCases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "PatientTeeth",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName_Ar",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName_En",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "PatientDebts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "PatientAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "LabRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "CaseTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientTeeth",
                table: "PatientTeeth",
                columns: new[] { "ToothId", "PatientId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_PatientId",
                table: "TreatmentCases",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTeeth_PatientId",
                table: "PatientTeeth",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_BranchId",
                table: "Patients",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Phone",
                table: "Patients",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDebts_PatientId",
                table: "PatientDebts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAttachments_PatientId",
                table: "PatientAttachments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabRequests_PatientId",
                table: "LabRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PatientId",
                table: "Invoices",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTransfers_PatientId",
                table: "CaseTransfers",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseTransfers_Patients_PatientId",
                table: "CaseTransfers",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Patients_PatientId",
                table: "Invoices",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabRequests_Patients_PatientId",
                table: "LabRequests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAttachments_Patients_PatientId",
                table: "PatientAttachments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDebts_Patients_PatientId",
                table: "PatientDebts",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Branches_BranchId",
                table: "Patients",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTeeth_Patients_PatientId",
                table: "PatientTeeth",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentCases_Patients_PatientId",
                table: "TreatmentCases",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
