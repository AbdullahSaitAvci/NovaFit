using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class MemberProfile
    {
        [Key]
        public int MemberProfileId { get; set; }

        // Identity kullanıcısının Id'si (String olarak tutulur)
        public string UserId { get; set; } = null!;

        [Display(Name = "Boy (cm)")]
        [Range(100, 250, ErrorMessage = "Lütfen geçerli bir boy giriniz.")]
        public int? HeightCm { get; set; }

        [Display(Name = "Kilo (kg)")]
        [Range(30, 300, ErrorMessage = "Lütfen geçerli bir kilo giriniz.")]
        public float? WeightKg { get; set; }

        [Display(Name = "Vücut Tipi")]
        public string? BodyType { get; set; }      // Ektomorf, Mezomorf...

        [Display(Name = "Hedef")]
        public string? Goal { get; set; }          // Kilo verme, Kas kazanma...
    }
}