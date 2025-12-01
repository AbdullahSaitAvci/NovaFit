using Microsoft.EntityFrameworkCore; 
using NovaFit.Models;

namespace NovaFit.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Buraya yazdığımız her bir DbSet, veri tabanında bir tabloya karşılık gelmektedir.

        public DbSet<FitnessService> FitnessServices { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerSpecialization> TrainerSpecializations { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MemberProfile> MemberProfiles { get; set; }
        public DbSet<AiRecommendation> AiRecommendations { get; set; }

    }
}