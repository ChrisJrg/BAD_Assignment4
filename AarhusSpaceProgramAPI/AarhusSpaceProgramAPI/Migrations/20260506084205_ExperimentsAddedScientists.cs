using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgramAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExperimentsAddedScientists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperimentId",
                table: "Missions");

            migrationBuilder.AddColumn<int>(
                name: "ScientistId",
                table: "Experiment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experiment_ScientistId",
                table: "Experiment",
                column: "ScientistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiment_Scientists_ScientistId",
                table: "Experiment",
                column: "ScientistId",
                principalTable: "Scientists",
                principalColumn: "ScientistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiment_Scientists_ScientistId",
                table: "Experiment");

            migrationBuilder.DropIndex(
                name: "IX_Experiment_ScientistId",
                table: "Experiment");

            migrationBuilder.DropColumn(
                name: "ScientistId",
                table: "Experiment");

            migrationBuilder.AddColumn<int>(
                name: "ExperimentId",
                table: "Missions",
                type: "int",
                nullable: true);
        }
    }
}
