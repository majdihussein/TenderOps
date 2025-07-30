using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class EditPartnerTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Partner_PartnerId",
                table: "Tenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Partner",
                table: "Partner");

            migrationBuilder.RenameTable(
                name: "Partner",
                newName: "Partners");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Partners",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Partners",
                table: "Partners",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ApplicationUserId",
                table: "Partners",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Partners_PartnerId",
                table: "Tenders",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Partners_PartnerId",
                table: "Tenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Partners",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ApplicationUserId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Partners");

            migrationBuilder.RenameTable(
                name: "Partners",
                newName: "Partner");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Partner",
                table: "Partner",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Partner_PartnerId",
                table: "Tenders",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
