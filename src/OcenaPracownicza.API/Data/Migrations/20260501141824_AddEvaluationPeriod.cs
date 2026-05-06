using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluationCriteria");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "Regulation",
                table: "EvaluationPeriods",
                newName: "RegulationVersion");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "EvaluationPeriods",
                newName: "IsClosed");

            migrationBuilder.AddColumn<Guid>(
                name: "EvaluationPeriodId",
                table: "Achievements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_EvaluationPeriodId",
                table: "Achievements",
                column: "EvaluationPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_EvaluationPeriods_EvaluationPeriodId",
                table: "Achievements",
                column: "EvaluationPeriodId",
                principalTable: "EvaluationPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_EvaluationPeriods_EvaluationPeriodId",
                table: "Achievements");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_EvaluationPeriodId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "EvaluationPeriodId",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "RegulationVersion",
                table: "EvaluationPeriods",
                newName: "Regulation");

            migrationBuilder.RenameColumn(
                name: "IsClosed",
                table: "EvaluationPeriods",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EvaluationCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluationPeriodId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluationPeriodId = table.Column<int>(type: "int", nullable: false),
                    MinimumScore = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationCriteria_EvaluationPeriods_EvaluationPeriodId1",
                        column: x => x.EvaluationPeriodId1,
                        principalTable: "EvaluationPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_EvaluationPeriodId1",
                table: "EvaluationCriteria",
                column: "EvaluationPeriodId1");
        }
    }
}
