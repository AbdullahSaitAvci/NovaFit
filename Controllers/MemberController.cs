using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;

namespace NovaFit.Controllers
{
    // Sadece "Member" rolüne sahip kullanıcılar erişebilir
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Member/Index (Randevularım)
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)             // View'da Resim ve İsim için gerekli
                .Include(a => a.FitnessService)      // View'da Hizmet Adı için gerekli
                .Include(a => a.TrainerAvailability) // Arka planda logic kontrolü için gerekli olabilir
                .Where(a => a.MemberUserId == userId)
                .OrderByDescending(a => a.StartTime) // Gelecek ve en yeni randevular üstte
                .ToListAsync();

            return View(appointments);
        }

        // POST: Member/CancelAppointment
        [HttpPost]
        [ValidateAntiForgeryToken] // Güvenlik önlemi (Form saldırılarına karşı)
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Randevuyu ve ilişkili Availability tablosunu çekiyoruz
            var appointment = await _context.Appointments
                .Include(a => a.TrainerAvailability)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.MemberUserId == userId);

            // 1. Randevu bulunamadıysa veya başkasına aitse
            if (appointment == null)
            {
                return NotFound();
            }

            // 2. Geçmiş randevu kontrolü
            if (appointment.StartTime < DateTime.Now)
            {
                TempData["Error"] = "Geçmiş tarihli randevular iptal edilemez.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Zaten iptal edilmişse işlem yapma
            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                TempData["Info"] = "Bu randevu zaten iptal edilmiş.";
                return RedirectToAction(nameof(Index));
            }

            // --- İPTAL İŞLEMİ ---
            appointment.Status = AppointmentStatus.Cancelled;

            // Kontenjan/Müsaitlik Yönetimi:
            // Eğer randevu iptal edilirse, hocanın o saatteki müsaitliği tekrar "Boş" (IsFull = false) hale gelir.
            if (appointment.TrainerAvailability != null)
            {
                // Eğer dolu olarak işaretlendiyse, tekrar boşa çıkar
                if (appointment.TrainerAvailability.IsFull)
                {
                    appointment.TrainerAvailability.IsFull = false;
                    _context.Update(appointment.TrainerAvailability);
                }
            }

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Info"] = "Randevunuz başarıyla iptal edildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}