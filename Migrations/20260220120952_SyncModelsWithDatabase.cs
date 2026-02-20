using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaFit.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelsWithDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // İÇİNİ TAMAMEN BOŞALTTIK. 
            // Böylece veritabanında SQL komutu çalıştırmayacak, 
            // sadece EF Core'un snapshot'ı güncellenmiş olacak.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
