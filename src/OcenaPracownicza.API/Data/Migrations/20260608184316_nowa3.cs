using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class nowa3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EvaluationPeriodId",
                table: "AchievementElements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AchievementElements_EvaluationPeriodId",
                table: "AchievementElements",
                column: "EvaluationPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementElements_EvaluationPeriods_EvaluationPeriodId",
                table: "AchievementElements",
                column: "EvaluationPeriodId",
                principalTable: "EvaluationPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AchievementElements_EvaluationPeriods_EvaluationPeriodId",
                table: "AchievementElements");

            migrationBuilder.DropIndex(
                name: "IX_AchievementElements_EvaluationPeriodId",
                table: "AchievementElements");

            migrationBuilder.DropColumn(
                name: "EvaluationPeriodId",
                table: "AchievementElements");
        }
    }
}
