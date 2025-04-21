using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixedAPI.Migrations
{
    /// <inheritdoc />
    public partial class ContactFormForCompany_Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "ContactFormForCompanies",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ContactFormForCompanies",
                newName: "CompanyName");
        }
    }
}
