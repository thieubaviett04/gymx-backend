using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPenaltyRuleCodeAndLineage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SeverityLevel",
                table: "penalty_rules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "penalty_rules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "penalty_rules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousRuleId",
                table: "penalty_rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RuleCode",
                table: "penalty_rules",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_penalty_rules_PreviousRuleId",
                table: "penalty_rules",
                column: "PreviousRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_penalty_rules_RuleCode",
                table: "penalty_rules",
                column: "RuleCode",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_penalty_rules_penalty_rules_PreviousRuleId",
                table: "penalty_rules",
                column: "PreviousRuleId",
                principalTable: "penalty_rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_penalty_rules_penalty_rules_PreviousRuleId",
                table: "penalty_rules");

            migrationBuilder.DropIndex(
                name: "IX_penalty_rules_PreviousRuleId",
                table: "penalty_rules");

            migrationBuilder.DropIndex(
                name: "IX_penalty_rules_RuleCode",
                table: "penalty_rules");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "penalty_rules");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "penalty_rules");

            migrationBuilder.DropColumn(
                name: "PreviousRuleId",
                table: "penalty_rules");

            migrationBuilder.DropColumn(
                name: "RuleCode",
                table: "penalty_rules");

            migrationBuilder.AlterColumn<string>(
                name: "SeverityLevel",
                table: "penalty_rules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
