using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAvailabilitySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "TrainerAvailabilities");

            migrationBuilder.RenameColumn(
                name: "IsBooked",
                table: "TrainerAvailabilities",
                newName: "IsFull");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "TrainerAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FitnessServiceId",
                table: "TrainerAvailabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAvailabilities_FitnessServiceId",
                table: "TrainerAvailabilities",
                column: "FitnessServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerAvailabilities_FitnessServices_FitnessServiceId",
                table: "TrainerAvailabilities",
                column: "FitnessServiceId",
                principalTable: "FitnessServices",
                principalColumn: "FitnessServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerAvailabilities_FitnessServices_FitnessServiceId",
                table: "TrainerAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_TrainerAvailabilities_FitnessServiceId",
                table: "TrainerAvailabilities");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "TrainerAvailabilities");

            migrationBuilder.DropColumn(
                name: "FitnessServiceId",
                table: "TrainerAvailabilities");

            migrationBuilder.RenameColumn(
                name: "IsFull",
                table: "TrainerAvailabilities",
                newName: "IsBooked");

            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "TrainerAvailabilities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
