using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrainerAvailabilityToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerAvailabilities_Trainers_TrainerId",
                table: "TrainerAvailabilities");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "TrainerAvailabilities");

            migrationBuilder.AlterColumn<int>(
                name: "TrainerId",
                table: "TrainerAvailabilities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "TrainerAvailabilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "TrainerAvailabilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerAvailabilities_Trainers_TrainerId",
                table: "TrainerAvailabilities",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "TrainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerAvailabilities_Trainers_TrainerId",
                table: "TrainerAvailabilities");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "TrainerAvailabilities");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "TrainerAvailabilities");

            migrationBuilder.AlterColumn<int>(
                name: "TrainerId",
                table: "TrainerAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "TrainerAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerAvailabilities_Trainers_TrainerId",
                table: "TrainerAvailabilities",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "TrainerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
