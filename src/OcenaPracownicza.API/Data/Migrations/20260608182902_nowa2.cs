using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OcenaPracownicza.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class nowa2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AchievementElementId",
                table: "Achievements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_AchievementElementId",
                table: "Achievements",
                column: "AchievementElementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_AchievementElements_AchievementElementId",
                table: "Achievements",
                column: "AchievementElementId",
                principalTable: "AchievementElements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_AchievementElements_AchievementElementId",
                table: "Achievements");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_AchievementElementId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "AchievementElementId",
                table: "Achievements");
        }
    }
}
