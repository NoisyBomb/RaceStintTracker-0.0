using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceStintTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceDurationToRace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "RaceDuration",
                table: "Races",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaceDuration",
                table: "Races");
        }
    }
}
