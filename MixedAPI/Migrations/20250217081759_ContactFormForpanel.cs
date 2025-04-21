using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixedAPI.Migrations
{
    /// <inheritdoc />
    public partial class ContactFormForpanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactFormForPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ResponseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedDepartment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeratorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactFormForPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactFormForPanels_ContactForms_Id",
                        column: x => x.Id,
                        principalTable: "ContactForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactFormForPanels");
        }
    }
}
