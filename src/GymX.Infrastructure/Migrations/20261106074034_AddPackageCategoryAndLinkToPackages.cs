using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageCategoryAndLinkToPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "packages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "package_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_package_categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_packages_CategoryId",
                table: "packages",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_package_categories_Code",
                table: "package_categories",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_package_categories_Name",
                table: "package_categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_package_categories_Status",
                table: "package_categories",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_packages_package_categories_CategoryId",
                table: "packages",
                column: "CategoryId",
                principalTable: "package_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_packages_package_categories_CategoryId",
                table: "packages");

            migrationBuilder.DropTable(
                name: "package_categories");

            migrationBuilder.DropIndex(
                name: "IX_packages_CategoryId",
                table: "packages");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "packages");
        }
    }
}
