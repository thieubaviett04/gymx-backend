using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMembershipPackageToPackageAndRefactorPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "membership_packages",
                newName: "packages");

            migrationBuilder.AddColumn<int>(
                name: "DurationMonths",
                table: "packages",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousPackageId",
                table: "packages",
                type: "uuid",
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_membership_packages_PackageCode",
                table: "packages");

            migrationBuilder.DropIndex(
                name: "IX_membership_packages_Status",
                table: "packages");

            migrationBuilder.CreateIndex(
                name: "IX_packages_PackageCode",
                table: "packages",
                column: "PackageCode",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_packages_PreviousPackageId",
                table: "packages",
                column: "PreviousPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_packages_Status",
                table: "packages",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_packages_packages_PreviousPackageId",
                table: "packages",
                column: "PreviousPackageId",
                principalTable: "packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.CreateIndex(
                name: "IX_member_packages_PackageId",
                table: "member_packages",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_member_packages_members_MemberId",
                table: "member_packages",
                column: "MemberId",
                principalTable: "members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_member_packages_packages_PackageId",
                table: "member_packages",
                column: "PackageId",
                principalTable: "packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_member_packages_members_MemberId",
                table: "member_packages");

            migrationBuilder.DropForeignKey(
                name: "FK_member_packages_packages_PackageId",
                table: "member_packages");

            migrationBuilder.DropIndex(
                name: "IX_member_packages_PackageId",
                table: "member_packages");

            migrationBuilder.DropForeignKey(
                name: "FK_packages_packages_PreviousPackageId",
                table: "packages");

            migrationBuilder.DropIndex(
                name: "IX_packages_PackageCode",
                table: "packages");

            migrationBuilder.DropIndex(
                name: "IX_packages_PreviousPackageId",
                table: "packages");

            migrationBuilder.DropIndex(
                name: "IX_packages_Status",
                table: "packages");

            migrationBuilder.DropColumn(
                name: "DurationMonths",
                table: "packages");

            migrationBuilder.DropColumn(
                name: "PreviousPackageId",
                table: "packages");

            migrationBuilder.RenameTable(
                name: "packages",
                newName: "membership_packages");

            migrationBuilder.CreateIndex(
                name: "IX_membership_packages_PackageCode",
                table: "membership_packages",
                column: "PackageCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_membership_packages_Status",
                table: "membership_packages",
                column: "Status");
        }
    }
}
