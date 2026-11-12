using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    public partial class AddPayslipSalaryBreakdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "payslips",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PtSessionsCount",
                table: "payslips",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PtTeachingAmount",
                table: "payslips",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesRevenue",
                table: "payslips",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "PtSessionsCount",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "PtTeachingAmount",
                table: "payslips");

            migrationBuilder.DropColumn(
                name: "SalesRevenue",
                table: "payslips");
        }
    }
}
