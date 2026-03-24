using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgramAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Astronauts",
                columns: table => new
                {
                    AstronautId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayGrade = table.Column<double>(type: "float", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EXPInSim = table.Column<double>(type: "float", nullable: false),
                    EXPInSpace = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Astronauts", x => x.AstronautId);
                });

            migrationBuilder.CreateTable(
                name: "CelestialBodies",
                columns: table => new
                {
                    CelestialBodyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    Composition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentPlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => 
                {
                    table.PrimaryKey("PK_CelestialBodies", x => x.CelestialBodyId);
                    table.ForeignKey(
                        name: "FK_CelestialBodies_CelestialBodies_ParentPlanetId",
                        column: x => x.ParentPlanetId,
                        principalTable: "CelestialBodies",
                        principalColumn: "CelestialBodyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaunchPads",
                columns: table => new
                {
                    LaunchPadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxWeight = table.Column<double>(type: "float", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaunchPads", x => x.LaunchPadId);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    ManagerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.ManagerId);
                });

            migrationBuilder.CreateTable(
                name: "Rockets",
                columns: table => new
                {
                    RocketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    CrewCapacity = table.Column<int>(type: "int", nullable: false),
                    Stages = table.Column<int>(type: "int", nullable: false),
                    FuelCapacity = table.Column<double>(type: "float", nullable: false),
                    PayloadCapacity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rockets", x => x.RocketId);
                });

            migrationBuilder.CreateTable(
                name: "Scientists",
                columns: table => new
                {
                    ScientistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scientists", x => x.ScientistId);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    MissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MissionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaunchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RocketId = table.Column<int>(type: "int", nullable: false),
                    LaunchpPadId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    TargetBodyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.MissionId);
                    table.ForeignKey(
                        name: "FK_Missions_CelestialBodies_TargetBodyId",
                        column: x => x.TargetBodyId,
                        principalTable: "CelestialBodies",
                        principalColumn: "CelestialBodyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_LaunchPads_LaunchpPadId",
                        column: x => x.LaunchpPadId,
                        principalTable: "LaunchPads",
                        principalColumn: "LaunchPadId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "ManagerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Rockets_RocketId",
                        column: x => x.RocketId,
                        principalTable: "Rockets",
                        principalColumn: "RocketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AstronautMission",
                columns: table => new
                {
                    AstronautsAstronautId = table.Column<int>(type: "int", nullable: false),
                    MissionsMissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstronautMission", x => new { x.AstronautsAstronautId, x.MissionsMissionId });
                    table.ForeignKey(
                        name: "FK_AstronautMission_Astronauts_AstronautsAstronautId",
                        column: x => x.AstronautsAstronautId,
                        principalTable: "Astronauts",
                        principalColumn: "AstronautId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AstronautMission_Missions_MissionsMissionId",
                        column: x => x.MissionsMissionId,
                        principalTable: "Missions",
                        principalColumn: "MissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionScientist",
                columns: table => new
                {
                    MissionsMissionId = table.Column<int>(type: "int", nullable: false),
                    ScientistsScientistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionScientist", x => new { x.MissionsMissionId, x.ScientistsScientistId });
                    table.ForeignKey(
                        name: "FK_MissionScientist_Missions_MissionsMissionId",
                        column: x => x.MissionsMissionId,
                        principalTable: "Missions",
                        principalColumn: "MissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissionScientist_Scientists_ScientistsScientistId",
                        column: x => x.ScientistsScientistId,
                        principalTable: "Scientists",
                        principalColumn: "ScientistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AstronautMission_MissionsMissionId",
                table: "AstronautMission",
                column: "MissionsMissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CelestialBodies_ParentPlanetId",
                table: "CelestialBodies",
                column: "ParentPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_LaunchpPadId",
                table: "Missions",
                column: "LaunchpPadId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_ManagerId",
                table: "Missions",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_RocketId",
                table: "Missions",
                column: "RocketId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_TargetBodyId",
                table: "Missions",
                column: "TargetBodyId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionScientist_ScientistsScientistId",
                table: "MissionScientist",
                column: "ScientistsScientistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AstronautMission");

            migrationBuilder.DropTable(
                name: "MissionScientist");

            migrationBuilder.DropTable(
                name: "Astronauts");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Scientists");

            migrationBuilder.DropTable(
                name: "CelestialBodies");

            migrationBuilder.DropTable(
                name: "LaunchPads");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "Rockets");
        }
    }
}
