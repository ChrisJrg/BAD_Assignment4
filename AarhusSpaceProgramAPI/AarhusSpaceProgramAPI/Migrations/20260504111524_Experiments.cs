using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgramAPI.Migrations
{
    /// <inheritdoc />
    public partial class Experiments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperimentId",
                table: "Missions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Experiment",
                columns: table => new
                {
                    ExperimentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperimentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiment", x => x.ExperimentId);
                    table.ForeignKey(
                        name: "FK_Experiment_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "MissionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Experiment_MissionId",
                table: "Experiment",
                column: "MissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Experiment");

            migrationBuilder.DropColumn(
                name: "ExperimentId",
                table: "Missions");
        }
    }
}
