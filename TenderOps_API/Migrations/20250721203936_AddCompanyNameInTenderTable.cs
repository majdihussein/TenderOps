using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenderOps_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyNameInTenderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Tenders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Tenders");
        }
    }
}
