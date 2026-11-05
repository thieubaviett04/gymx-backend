using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSalaryCoefficientWithPricePerSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryCoefficient",
                table: "trainers");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerSession",
                table: "trainers",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerSession",
                table: "trainers");

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryCoefficient",
                table: "trainers",
                type: "numeric(5,2)",
                nullable: true);
        }
    }
}
