using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class EditInModelClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ApplicationUserId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Partners");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PartnerId",
                table: "AspNetUsers",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Partners_PartnerId",
                table: "AspNetUsers",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Partners_PartnerId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PartnerId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Partners",
                type: "nvarchar(450)",
                nullable: true);

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
