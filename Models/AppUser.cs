using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    // IdentityUser sınıfından miras alıyoruz çünkü kullanıcı yönetimi için gerekli özellikler (kullanıcı adı, şifre, email vb.) zaten orada tanımlı.
    public class AppUser : IdentityUser // Bu sınıf, uygulamanın kullanıcılarını temsil eder.
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string FullName { get; set; } = null!;

        // Yapay zeka ve randevu için gerekli bilgiler
        public int? Height { get; set; } // Boy
        public double? Weight { get; set; } // Kilo

        // Kullanıcının randevuları
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
