using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;
using NovaFit.Models;

namespace NovaFit.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) // base(options) ile üst sınıfın (IdentityDbContext) yapıcı metodunu çağırıyoruz.
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
