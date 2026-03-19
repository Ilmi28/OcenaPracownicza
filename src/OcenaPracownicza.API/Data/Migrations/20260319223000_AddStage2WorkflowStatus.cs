using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    public partial class AddStage2WorkflowStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Stage2Comment",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stage2ReviewedByUserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stage2ReviewedAtUtc",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage2Status",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Stage2Comment",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage2Status",
                table: "Achievements",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage2Comment",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2ReviewedByUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2ReviewedAtUtc",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2Status",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2Comment",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Stage2Status",
                table: "Achievements");
        }
    }
}
