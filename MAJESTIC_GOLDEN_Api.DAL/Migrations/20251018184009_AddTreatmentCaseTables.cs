using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAJESTIC_GOLDEN_Api.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentCaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreatmentCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CaseNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title_En = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title_Ar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description_En = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description_Ar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes_Ar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentCases_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentCases_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TreatmentCases_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseDoctors",
                columns: table => new
                {
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role_En = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role_Ar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseDoctors", x => new { x.CaseId, x.DoctorId });
                    table.ForeignKey(
                        name: "FK_CaseDoctors_TreatmentCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "TreatmentCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseDoctors_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseTreatments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes_Ar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreatmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTreatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseTreatments_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseTreatments_TreatmentCases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "TreatmentCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseTreatments_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseDoctors_DoctorId",
                table: "CaseDoctors",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTreatments_CaseId",
                table: "CaseTreatments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTreatments_DoctorId",
                table: "CaseTreatments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTreatments_ServiceId",
                table: "CaseTreatments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_BranchId",
                table: "TreatmentCases",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_CaseDate",
                table: "TreatmentCases",
                column: "CaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_CaseNumber",
                table: "TreatmentCases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_InvoiceId",
                table: "TreatmentCases",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_PatientId",
                table: "TreatmentCases",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCases_Status",
                table: "TreatmentCases",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseDoctors");

            migrationBuilder.DropTable(
                name: "CaseTreatments");

            migrationBuilder.DropTable(
                name: "TreatmentCases");
        }
    }
}
