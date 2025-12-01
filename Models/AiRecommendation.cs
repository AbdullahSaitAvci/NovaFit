using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class AiRecommendation
    {
        [Key]
        public int AiRecommendationId { get; set; }

        public string MemberUserId { get; set; } = null!;

        [Display(Name = "Girilen Veriler")]
        public string? InputDataJson { get; set; }      // Gönderilen veriler

        [Display(Name = "AI Önerisi")]
        public string? ResultText { get; set; }         // AI'ın cevabı

        [Display(Name = "Görsel")]
        public string? ResultImageUrl { get; set; }     // Dönen görsel varsa

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}