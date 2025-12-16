using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_AppUserId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "MemberUserId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_MemberUserId",
                table: "Appointments",
                column: "MemberUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_MemberUserId",
                table: "Appointments",
                column: "MemberUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_MemberUserId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_MemberUserId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "MemberUserId",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppUserId",
                table: "Appointments",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_AppUserId",
                table: "Appointments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
