using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeKpiManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kpi_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TargetRole = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TargetPtSessions = table.Column<int>(type: "integer", nullable: true),
                    TargetRevenue = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    TargetNewMembers = table.Column<int>(type: "integer", nullable: true),
                    TargetBonusAmount = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpi_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee_kpis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    KpiTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TargetPtSessions = table.Column<int>(type: "integer", nullable: true),
                    TargetRevenue = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    TargetNewMembers = table.Column<int>(type: "integer", nullable: true),
                    TargetBonusAmount = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_kpis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_kpis_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employee_kpis_kpi_templates_KpiTemplateId",
                        column: x => x.KpiTemplateId,
                        principalTable: "kpi_templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_kpis_EmployeeId_Month_Year",
                table: "employee_kpis",
                columns: new[] { "EmployeeId", "Month", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_kpis_KpiTemplateId",
                table: "employee_kpis",
                column: "KpiTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_kpis_Month_Year",
                table: "employee_kpis",
                columns: new[] { "Month", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_employee_kpis_Status",
                table: "employee_kpis",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_kpi_templates_Name",
                table: "kpi_templates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_kpi_templates_Status",
                table: "kpi_templates",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_kpi_templates_TargetRole",
                table: "kpi_templates",
                column: "TargetRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_kpis");

            migrationBuilder.DropTable(
                name: "kpi_templates");
        }
    }
}
