using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberIdToAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberId",
                table: "TrainerAvailabilities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "TrainerAvailabilities");
        }
    }
}
