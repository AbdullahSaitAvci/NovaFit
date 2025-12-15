using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class Trainer
    {
        [Key]
        public int TrainerId { get; set; } // Join işlemlerinde karışmasın diye TrainerId dedik

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        public string FullName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Geçerli bir E-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [Display(Name = "Biyografi")]
        [DataType(DataType.MultilineText)] // Kutucuğun büyük olması için
        public string? Bio { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string? Expertise { get; set; }

        [Display(Name = "Fotoğraf")]
        public string? ImageUrl { get; set; } // Antrenör fotosu için

        // --- İlişkiler ---
        public virtual ICollection<TrainerSpecialization> Specializations { get; set; } = new List<TrainerSpecialization>();
        public virtual ICollection<TrainerAvailability> TrainerAvailabilities { get; set; } = new List<TrainerAvailability>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}