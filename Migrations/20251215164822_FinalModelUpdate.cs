using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class FinalModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TrainerAvailabilityId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TrainerAvailabilityId",
                table: "Appointments",
                column: "TrainerAvailabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_TrainerAvailabilities_TrainerAvailabilityId",
                table: "Appointments",
                column: "TrainerAvailabilityId",
                principalTable: "TrainerAvailabilities",
                principalColumn: "TrainerAvailabilityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_TrainerAvailabilities_TrainerAvailabilityId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TrainerAvailabilityId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TrainerAvailabilityId",
                table: "Appointments");
        }
    }
}
