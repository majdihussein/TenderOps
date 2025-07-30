using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class AddTenderCategoryToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenderCategoryId",
                table: "Tenders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TenderCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_TenderCategoryId",
                table: "Tenders",
                column: "TenderCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_TenderCategory_TenderCategoryId",
                table: "Tenders",
                column: "TenderCategoryId",
                principalTable: "TenderCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_TenderCategory_TenderCategoryId",
                table: "Tenders");

            migrationBuilder.DropTable(
                name: "TenderCategory");

            migrationBuilder.DropIndex(
                name: "IX_Tenders_TenderCategoryId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "TenderCategoryId",
                table: "Tenders");
        }
    }
}
