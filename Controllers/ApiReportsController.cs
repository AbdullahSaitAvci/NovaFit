using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;
using System.Security.Claims;

namespace NovaFit.Controllers
{
    // API Route'u: /api/ApiReports
    [Route("api/[controller]")]
    [ApiController] // API davranışlarını etkinleştirir (otomatik 400 Bad Request yanıtı vb.)
    public class ApiReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Örnek: Tüm Antrenörleri Listeleme (GET /api/ApiReports/trainers)
        [HttpGet("trainers")]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetTrainers()
        {
            // İsteğe bağlı olarak sadece Admin veya yetkili kullanıcılara açabiliriz:
            // if (!User.IsInRole("Admin")) return Forbid(); 

            return await _context.Trainers.ToListAsync();
        }

        // Örnek: Belirli Bir Tarihte Uygun Antrenörleri Getirme
        // (GET /api/ApiReports/available-trainers?date=2026-01-01)
        [HttpGet("available-trainers")]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetAvailableTrainers([FromQuery] DateTime date)
        {
            if (date == default)
            {
                return BadRequest("Tarih parametresi (date) zorunludur.");
            }

            // Kullanıcıdan gelen tarihi saat/dakika bilgisi olmadan al (Sadece Gün)
            var targetDate = date.Date;

            // Belirtilen tarihte Müsaitlik (Availability) kaydı olan eğitmen ID'lerini bul.
            var availableTrainerIds = await _context.TrainerAvailabilities
                .Where(ta => ta.Date.Date == targetDate && ta.IsFull == false)
                .Select(ta => ta.TrainerId)
                .Distinct() // Birden fazla slotu olsa da ID'yi bir kez al
                .ToListAsync();

            // Bu ID'lere sahip eğitmen profillerini getir.
            var availableTrainers = await _context.Trainers
                .Where(t => availableTrainerIds.Contains(t.TrainerId))
                .ToListAsync();

            return availableTrainers;
        }

        // Örnek: Üye Randevularını Getirme (LINQ ile Filtreleme)
        // (GET /api/ApiReports/member-appointments?userId={GUID}&status=Pending)
        [HttpGet("member-appointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetMemberAppointments(
            [FromQuery] string userId,
            [FromQuery] AppointmentStatus? status) // Opsiyonel Durum Filtresi
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID (userId) parametresi zorunludur.");
            }

            // LINQ sorgusu ile veriyi filtreleme
            var query = _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.FitnessService)
                .Where(a => a.MemberUserId == userId); // Zorunlu kullanıcı filtresi

            // Eğer durum (status) filtresi varsa onu da uygularız
            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            // Sonucu sırala ve listele
            var appointments = await query
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();

            if (!appointments.Any())
            {
                // 200 OK döner, ama liste boştur.
                return Ok(new List<Appointment>());
            }

            return appointments;
        }
    }
}