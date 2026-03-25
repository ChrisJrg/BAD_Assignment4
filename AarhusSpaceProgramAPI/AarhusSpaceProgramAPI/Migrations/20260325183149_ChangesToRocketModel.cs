using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgramAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToRocketModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Missions_RocketId",
                table: "Missions");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_RocketId",
                table: "Missions",
                column: "RocketId",
                unique: true,
                filter: "[RocketId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Missions_RocketId",
                table: "Missions");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_RocketId",
                table: "Missions",
                column: "RocketId");
        }
    }
}
