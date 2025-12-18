using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovaFit.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        // İlişkiler

        // Randevuyu alan Üye (Identity User ID)
        [Display(Name = "Üye")]
        public string MemberUserId { get; set; } = null!;

        [ForeignKey("MemberUserId")]
        public virtual AppUser? MemberUser { get; set; } // Üyenin detaylarına (Ad, Soyad) erişmek için

        // Hangi Eğitmen?
        [Display(Name = "Eğitmen")]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }

        // Hangi Hizmet?
        [Display(Name = "Hizmet")]
        public int? FitnessServiceId { get; set; }

        [ForeignKey("FitnessServiceId")]
        public virtual FitnessService? FitnessService { get; set; }

        // Hangi Müsaitlik Slotuna Bağlı?
        [Display(Name = "Ders Saati Kaydı")]
        public int? TrainerAvailabilityId { get; set; }

        [ForeignKey("TrainerAvailabilityId")]
        public virtual TrainerAvailability? TrainerAvailability { get; set; }

        // Randevu Detayları

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Ücret")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
    public enum AppointmentStatus
    {
        [Display(Name = "Beklemede")]
        Pending,

        [Display(Name = "Onaylandı")]
        Confirmed,

        [Display(Name = "Tamamlandı")]
        Completed,

        [Display(Name = "İptal Edildi")]
        Cancelled
    }
}