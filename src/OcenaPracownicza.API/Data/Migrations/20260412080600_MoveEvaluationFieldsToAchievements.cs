using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveEvaluationFieldsToAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AchievementsSummary",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FinalScore",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Stage2ReviewedAtUtc",
                table: "Achievements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stage2ReviewedByUserId",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE a
                SET
                    a.Period = ISNULL(e.Period, ''),
                    a.FinalScore = ISNULL(e.FinalScore, ''),
                    a.AchievementsSummary = ISNULL(e.AchievementsSummary, ''),
                    a.Stage2Status = e.Stage2Status,
                    a.Stage2Comment = e.Stage2Comment,
                    a.Stage2ReviewedByUserId = e.Stage2ReviewedByUserId,
                    a.Stage2ReviewedAtUtc = e.Stage2ReviewedAtUtc
                FROM Achievements a
                INNER JOIN Employees e ON e.Id = a.EmployeeId
                """);

            migrationBuilder.DropColumn(
                name: "AchievementsSummary",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2Comment",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2ReviewedAtUtc",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2ReviewedByUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Stage2Status",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievementsSummary",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Stage2ReviewedAtUtc",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Stage2ReviewedByUserId",
                table: "Achievements");

            migrationBuilder.AddColumn<string>(
                name: "AchievementsSummary",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FinalScore",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stage2Comment",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stage2ReviewedAtUtc",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stage2ReviewedByUserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage2Status",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
