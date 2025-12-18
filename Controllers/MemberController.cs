using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;

namespace NovaFit.Controllers
{
    // Bu Controller'a sadece üyeler olanlar girebilir.

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

        // GET: Member/Index 
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var appointments = await _context.Appointments
                .Include(a => a.Trainer) // Antrenör bilgisi
                .Include(a => a.FitnessService) // Hizmet bilgisi
                .Include(a => a.TrainerAvailability) // Müsaitlik bilgisi
                .Where(a => a.MemberUserId == userId) // Sadece bu üyenin randevuları
                .OrderByDescending(a => a.StartTime) // En yeni tarih en üstte
                .ToListAsync();

            return View(appointments); // Views/Member/Index.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var userId = _userManager.GetUserId(User);

            var appointment = await _context.Appointments
                .Include(a => a.TrainerAvailability)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.MemberUserId == userId);

            if (appointment == null) return NotFound();

            if (appointment.StartTime < DateTime.Now)
            {
                TempData["Error"] = "Geçmiş randevular iptal edilemez.";
                return RedirectToAction(nameof(Index));
            }

            appointment.Status = AppointmentStatus.Cancelled;

            // Kontenjanı geri açar
            if (appointment.TrainerAvailability != null)
            {
                if (appointment.TrainerAvailability.IsFull)
                {
                    appointment.TrainerAvailability.IsFull = false;
                    _context.Update(appointment.TrainerAvailability);
                }
            }

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Info"] = "Randevunuz iptal edildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}