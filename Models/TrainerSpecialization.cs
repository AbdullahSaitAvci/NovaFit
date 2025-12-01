using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class TrainerSpecialization
    {
        [Key]
        public int TrainerSpecializationId { get; set; }

        public int TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }

        [Required(ErrorMessage = "Uzmanlık adı boş bırakılamaz.")]
        [Display(Name = "Uzmanlık Alanı")] // Örn: Pilates, Bodybuilding
        public string Name { get; set; } = null!;
    }
}