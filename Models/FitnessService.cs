using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // decimal için gerekli olabilir


namespace NovaFit.Models
{
    public class FitnessService
    {
        [Key]
        public int FitnessServiceId { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        [StringLength(50, ErrorMessage = "Hizmet adı en fazla 50 karakter olabilir.")]
        public string ServiceName { get; set; } // Örn: Pilates, Yoga

        [Display(Name = "Açıklama")]
        public string? Description { get; set; } 

        [Required(ErrorMessage = "Süre bilgisi zorunludur.")]
        [Display(Name = "Süre (Dakika)")]
       
        [Range(1, 180, ErrorMessage = "Süre 1 ile 180 dakika arasında olmalıdır.")]
        public int DurationMinutes { get; set; } 

        [Required(ErrorMessage = "Ücret bilgisi zorunludur.")]
        [Display(Name = "Ücret (TL)")]
        [Range(1, 10000, ErrorMessage = "Ücret en az 1 TL olmalıdır.")]
        [Column(TypeName = "decimal(18,2)")]  // Veritabanında doğru tip için

        public decimal Price { get; set; }

        // --- İlişkiler (Navigation Properties) ---
        // Bir hizmetin (Örn: Pilates) birden fazla randevusu olabilir.
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}