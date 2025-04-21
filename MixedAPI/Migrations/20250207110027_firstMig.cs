using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixedAPI.Migrations
{
    /// <inheritdoc />
    public partial class firstMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TitleAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailDescriptionAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailDescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailDescriptionRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetAudienceAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetAudienceEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetAudienceRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurriculumAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurriculumEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurriculumRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeaturesAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeaturesEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeaturesRu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramDurationAz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramDurationEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramDurationRu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Participants = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avantage = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactForms_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactForms_CourseId",
                table: "ContactForms",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DropTable(
                name: "CourseDetails");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
