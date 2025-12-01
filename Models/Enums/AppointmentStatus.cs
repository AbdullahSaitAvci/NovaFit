namespace NovaFit.Models
{
    public enum AppointmentStatus
    {
        Pending = 0,     // Onay bekliyor
        Approved = 1,    // Onaylandı
        Rejected = 2,    // Reddedildi
        Completed = 3,   // Tamamlandı
        Cancelled = 4    // İptal edildi
    }
}