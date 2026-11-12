using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{

    public partial class AddPayrollBonusesTable : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payroll_bonuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    BonusType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PayrollPeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_bonuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_bonuses_EmployeeId_Month_Year",
                table: "payroll_bonuses",
                columns: new[] { "EmployeeId", "Month", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_bonuses_Month_Year",
                table: "payroll_bonuses",
                columns: new[] { "Month", "Year" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payroll_bonuses");
        }
    }
}
