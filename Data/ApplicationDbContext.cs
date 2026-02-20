using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovaFit.Models;

namespace NovaFit.Data
{
    // IdentityDbContext kullanırken AppUser ve AppRole sınıflarını ve Key tipini (string) belirtiyoruz.
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // --- TABLOLAR (DbSet) ---
        public DbSet<FitnessService> FitnessServices { get; set; }
        public DbSet<Trainer> Trainers { get; set; }

        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<MemberProfile> MemberProfiles { get; set; }
        public DbSet<TrainerSpecialization> TrainerSpecializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AiRecommendation> AiRecommendations { get; set; }

        // İlişki Ayarlamaları
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Identity için ŞART

            // Appointment tablosu için hassasiyet ayarı 
            modelBuilder.Entity<Appointment>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
