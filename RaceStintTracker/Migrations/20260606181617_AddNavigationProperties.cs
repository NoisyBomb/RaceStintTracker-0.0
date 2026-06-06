using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceStintTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Stints_DriverId",
                table: "Stints",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Stints_RaceId",
                table: "Stints",
                column: "RaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stints_Drivers_DriverId",
                table: "Stints",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stints_Races_RaceId",
                table: "Stints",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stints_Drivers_DriverId",
                table: "Stints");

            migrationBuilder.DropForeignKey(
                name: "FK_Stints_Races_RaceId",
                table: "Stints");

            migrationBuilder.DropIndex(
                name: "IX_Stints_DriverId",
                table: "Stints");

            migrationBuilder.DropIndex(
                name: "IX_Stints_RaceId",
                table: "Stints");
        }
    }
}
