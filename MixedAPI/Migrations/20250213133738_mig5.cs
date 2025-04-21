using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MixedAPI.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "CourseDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseDetails_CourseId",
                table: "CourseDetails",
                column: "CourseId",
                unique: true,
                filter: "[CourseId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDetails_Courses_CourseId",
                table: "CourseDetails",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseDetails_Courses_CourseId",
                table: "CourseDetails");

            migrationBuilder.DropIndex(
                name: "IX_CourseDetails_CourseId",
                table: "CourseDetails");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseDetails");
        }
    }
}
