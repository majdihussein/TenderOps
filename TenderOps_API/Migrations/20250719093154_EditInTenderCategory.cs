using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class EditInTenderCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_TenderCategory_TenderCategoryId",
                table: "Tenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderCategory",
                table: "TenderCategory");

            migrationBuilder.RenameTable(
                name: "TenderCategory",
                newName: "TenderCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderCategories",
                table: "TenderCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_TenderCategories_TenderCategoryId",
                table: "Tenders",
                column: "TenderCategoryId",
                principalTable: "TenderCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_TenderCategories_TenderCategoryId",
                table: "Tenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderCategories",
                table: "TenderCategories");

            migrationBuilder.RenameTable(
                name: "TenderCategories",
                newName: "TenderCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderCategory",
                table: "TenderCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_TenderCategory_TenderCategoryId",
                table: "Tenders",
                column: "TenderCategoryId",
                principalTable: "TenderCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
