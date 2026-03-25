using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgramAPI.Migrations
{
    /// <inheritdoc />
    public partial class renamedattributeinmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Missions_CelestialBodies_TargetBodyId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_LaunchPads_LaunchpPadId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Managers_ManagerId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Rockets_RocketId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_LaunchpPadId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "LaunchpPadId",
                table: "Missions");

            migrationBuilder.AlterColumn<int>(
                name: "TargetBodyId",
                table: "Missions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RocketId",
                table: "Missions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "Missions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LaunchPadId",
                table: "Missions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Missions_LaunchPadId",
                table: "Missions",
                column: "LaunchPadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_CelestialBodies_TargetBodyId",
                table: "Missions",
                column: "TargetBodyId",
                principalTable: "CelestialBodies",
                principalColumn: "CelestialBodyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_LaunchPads_LaunchPadId",
                table: "Missions",
                column: "LaunchPadId",
                principalTable: "LaunchPads",
                principalColumn: "LaunchPadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Managers_ManagerId",
                table: "Missions",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Rockets_RocketId",
                table: "Missions",
                column: "RocketId",
                principalTable: "Rockets",
                principalColumn: "RocketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Missions_CelestialBodies_TargetBodyId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_LaunchPads_LaunchPadId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Managers_ManagerId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Rockets_RocketId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_LaunchPadId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "LaunchPadId",
                table: "Missions");

            migrationBuilder.AlterColumn<int>(
                name: "TargetBodyId",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RocketId",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaunchpPadId",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Missions_LaunchpPadId",
                table: "Missions",
                column: "LaunchpPadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_CelestialBodies_TargetBodyId",
                table: "Missions",
                column: "TargetBodyId",
                principalTable: "CelestialBodies",
                principalColumn: "CelestialBodyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_LaunchPads_LaunchpPadId",
                table: "Missions",
                column: "LaunchpPadId",
                principalTable: "LaunchPads",
                principalColumn: "LaunchPadId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Managers_ManagerId",
                table: "Missions",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Rockets_RocketId",
                table: "Missions",
                column: "RocketId",
                principalTable: "Rockets",
                principalColumn: "RocketId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
