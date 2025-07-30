using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class EditInApplicationUserToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_AspNetUsers_ApplicationUserId",
                table: "Partners",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
