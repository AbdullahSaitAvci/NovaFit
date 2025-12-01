using System.ComponentModel.DataAnnotations;

namespace NovaFit.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int TrainerAvailabilityId { get; set; }

        public int TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }

        [Required]
        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }   // Pazartesi, Salı...

        [Required]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }    // 10:00

        [Required]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }      // 12:00
    }
}