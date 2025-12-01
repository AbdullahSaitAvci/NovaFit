using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        // İlişkiler
        public int TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }

        public int FitnessServiceId { get; set; }
        public virtual FitnessService FitnessService { get; set; }

        public string MemberUserId { get; set; } = null!;  // Randevuyu alan üye

        [Required]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Ücret")]
        public decimal Price { get; set; }

        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }
}