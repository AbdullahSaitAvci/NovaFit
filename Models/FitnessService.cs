using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class FitnessService
    {
        [Key]
        public int FitnessServiceId { get; set; } // 'Id' yerine SınıfAdıId kullanımı karışıklığı önler.

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        [StringLength(50, ErrorMessage = "Hizmet adı en fazla 50 karakter olabilir.")]
        public string ServiceName { get; set; } // Örn: Pilates, Yoga

        [Display(Name = "Açıklama")]
        public string? Description { get; set; } // Soru işareti (?) boş geçilebilir demek.

        [Required(ErrorMessage = "Süre bilgisi zorunludur.")]
        [Display(Name = "Süre (Dakika)")]
        [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")]
        public int DurationMinutes { get; set; } // 'Duration' yerine 'DurationMinutes' daha açıklayıcı.

        [Required(ErrorMessage = "Ücret bilgisi zorunludur.")]
        [Display(Name = "Ücret (TL)")]
        public decimal Price { get; set; }

        // --- İlişkiler (Navigation Properties) ---
        // Bir hizmetin (Örn: Pilates) birden fazla randevusu olabilir.
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}