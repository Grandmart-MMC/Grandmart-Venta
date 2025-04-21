using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixedAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixIdentityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactFormForPanels_ContactForms_Id",
                table: "ContactFormForPanels");

            migrationBuilder.AddColumn<int>(
                name: "ContactFormId",
                table: "ContactFormForPanels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContactFormForPanels_ContactFormId",
                table: "ContactFormForPanels",
                column: "ContactFormId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactFormForPanels_ContactForms_ContactFormId",
                table: "ContactFormForPanels",
                column: "ContactFormId",
                principalTable: "ContactForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactFormForPanels_ContactForms_ContactFormId",
                table: "ContactFormForPanels");

            migrationBuilder.DropIndex(
                name: "IX_ContactFormForPanels_ContactFormId",
                table: "ContactFormForPanels");

            migrationBuilder.DropColumn(
                name: "ContactFormId",
                table: "ContactFormForPanels");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactFormForPanels_ContactForms_Id",
                table: "ContactFormForPanels",
                column: "Id",
                principalTable: "ContactForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
