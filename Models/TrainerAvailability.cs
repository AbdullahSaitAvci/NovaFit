using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovaFit.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int TrainerAvailabilityId { get; set; }

        [Display(Name = "Eğitmen")]
        public int? TrainerId { get; set; }
        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }

        // Bu derse kayıt yaptıranların listesi (Çoklu kayıt için şart)
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Eğer hoca bu saati "Özel bir ders" için açtıysa burada belirtebiliriz.
        // Null ise "Genel Müsaitlik" demektir.
        [Display(Name = "Ders Tipi / Hizmet")]
        public int? FitnessServiceId { get; set; }
        [ForeignKey("FitnessServiceId")]
        public virtual FitnessService? FitnessService { get; set; }

        // Ders Saati Bilgileri

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [Display(Name = "Ders Tarihi")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // Kapasite ve Doluluk Durumu

        [Required]
        [Display(Name = "Kontenjan")]
        [Range(1, 100, ErrorMessage = "Kontenjan en az 1, en fazla 100 olabilir.")]
        public int Capacity { get; set; } = 1; // Varsayılan 1 (Bireysel Ders)

        [Display(Name = "Doluluk Durumu")]
        public bool IsFull { get; set; } = false; // Kontenjan doldu mu?
    }
}