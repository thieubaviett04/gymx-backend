using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    public partial class UpdateSalaryFormulaSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "salary_formulas",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "salary_formulas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerPtSession",
                table: "salary_formulas",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesCommissionPercent",
                table: "salary_formulas",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StandardWorkDays",
                table: "salary_formulas",
                type: "integer",
                nullable: false,
                defaultValue: 26);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "salary_formulas",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_salary_formulas_EffectiveDate",
                table: "salary_formulas",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_salary_formulas_Name",
                table: "salary_formulas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_formulas_Status",
                table: "salary_formulas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_salary_formulas_TargetRole",
                table: "salary_formulas",
                column: "TargetRole");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_salary_formulas_EffectiveDate",
                table: "salary_formulas");

            migrationBuilder.DropIndex(
                name: "IX_salary_formulas_Name",
                table: "salary_formulas");

            migrationBuilder.DropIndex(
                name: "IX_salary_formulas_Status",
                table: "salary_formulas");

            migrationBuilder.DropIndex(
                name: "IX_salary_formulas_TargetRole",
                table: "salary_formulas");

            migrationBuilder.DropColumn(
                name: "PricePerPtSession",
                table: "salary_formulas");

            migrationBuilder.DropColumn(
                name: "SalesCommissionPercent",
                table: "salary_formulas");

            migrationBuilder.DropColumn(
                name: "StandardWorkDays",
                table: "salary_formulas");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "salary_formulas");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "salary_formulas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "salary_formulas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
