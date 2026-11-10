using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddViolationCodeAndDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "violations",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "violations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "violations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledBy",
                table: "violations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceUrl",
                table: "violations",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "violations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ViolationCode",
                table: "violations",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_violations_EmployeeId_OccurredAt",
                table: "violations",
                columns: new[] { "EmployeeId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_violations_Status",
                table: "violations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_violations_ViolationCode",
                table: "violations",
                column: "ViolationCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_violations_employee_attendance_records_AttendanceRecordId",
                table: "violations",
                column: "AttendanceRecordId",
                principalTable: "employee_attendance_records",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_violations_employees_EmployeeId",
                table: "violations",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_violations_penalty_rules_PenaltyRuleId",
                table: "violations",
                column: "PenaltyRuleId",
                principalTable: "penalty_rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_violations_employee_attendance_records_AttendanceRecordId",
                table: "violations");

            migrationBuilder.DropForeignKey(
                name: "FK_violations_employees_EmployeeId",
                table: "violations");

            migrationBuilder.DropForeignKey(
                name: "FK_violations_penalty_rules_PenaltyRuleId",
                table: "violations");

            migrationBuilder.DropIndex(
                name: "IX_violations_EmployeeId_OccurredAt",
                table: "violations");

            migrationBuilder.DropIndex(
                name: "IX_violations_Status",
                table: "violations");

            migrationBuilder.DropIndex(
                name: "IX_violations_ViolationCode",
                table: "violations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "violations");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "violations");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "violations");

            migrationBuilder.DropColumn(
                name: "EvidenceUrl",
                table: "violations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "violations");

            migrationBuilder.DropColumn(
                name: "ViolationCode",
                table: "violations");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "violations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
