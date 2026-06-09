using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class nowa4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "AchievementElements",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "AchievementElements",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "Activity",
                table: "AchievementElements",
                newName: "ActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "AchievementElements",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "AchievementElements",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "AchievementElements",
                newName: "Activity");
        }
    }
}
