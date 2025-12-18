using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class MakeServiceIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_FitnessServices_FitnessServiceId",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "FitnessServiceId",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_FitnessServices_FitnessServiceId",
                table: "Appointments",
                column: "FitnessServiceId",
                principalTable: "FitnessServices",
                principalColumn: "FitnessServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_FitnessServices_FitnessServiceId",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "FitnessServiceId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_FitnessServices_FitnessServiceId",
                table: "Appointments",
                column: "FitnessServiceId",
                principalTable: "FitnessServices",
                principalColumn: "FitnessServiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
