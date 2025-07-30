using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Tenders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Partner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partner", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_PartnerId",
                table: "Tenders",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Partner_PartnerId",
                table: "Tenders",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Partner_PartnerId",
                table: "Tenders");

            migrationBuilder.DropTable(
                name: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_Tenders_PartnerId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Tenders");
        }
    }
}
