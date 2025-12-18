using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;
using System.Security.Claims; // Kullanıcı ID'si almak için gerekli

namespace NovaFit.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<AppUser> _userManager;

        public TrainersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<AppUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Trainers
        public async Task<IActionResult> Index(string searchString)
        {
            // İlişkili tabloyu (TrainerSpecializations) dahil ediyoruz
            var trainers = _context.Trainers
                .Include(t => t.TrainerSpecializations)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Arama terimini küçük harfe çevir (Büyük/Küçük harf duyarlılığını kaldırır)
                string searchLower = searchString.ToLower();

                // Filtreleme Mantığı:
                trainers = trainers.Where(t =>
                    // String kutusunda var mı? (Null değilse ve içeriyorsa)
                    (t.Expertise != null && t.Expertise.ToLower().Contains(searchLower))
                    ||
                    // Veya tabloda (TrainerSpecializations) bu isimde bir uzmanlık var mı?
                    t.TrainerSpecializations.Any(ts => ts.Name.ToLower().Contains(searchLower))
                );
            }

            return View(await trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.TrainerAvailabilities)
                .ThenInclude(ta => ta.Appointments) // <-- YENİ EKLENEN KRİTİK SATIR
                .FirstOrDefaultAsync(m => m.TrainerId == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // Üye Randevu Yönetim İşlemleri

        // Üyenin Randevularını Listele
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)           // Hangi hoca?
                .Include(a => a.FitnessService)    // Hangi ders?
                .Include(a => a.TrainerAvailability) // Tarih bilgileri için (Opsiyonel, zaten StartTime var)
                .Where(a => a.MemberUserId == userId)
                .OrderByDescending(a => a.StartTime) // En yeni tarih en üstte
                .ToListAsync();

            return View(appointments);
        }

        // Üyenin Randevusunu İptal Etmesi
        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CancelMyAppointment(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Randevuyu bul (Sadece bu kullanıcıya aitse!)
            var appointment = await _context.Appointments
                .Include(a => a.TrainerAvailability)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.MemberUserId == userId);

            if (appointment == null) return NotFound();

            // Geçmiş randevu iptal edilemez (Opsiyonel kural)
            if (appointment.StartTime < DateTime.Now)
            {
                TempData["Error"] = "Geçmiş randevular iptal edilemez.";
                return RedirectToAction(nameof(MyAppointments));
            }

            // Durumu İptal Yap
            appointment.Status = AppointmentStatus.Cancelled;

            // KONTENJANI GERİ AÇMA MANTIĞI
            if (appointment.TrainerAvailability != null)
            {
                // Eğer slot "Full" ise ve biz çıkıyorsak, artık yer var demektir.
                if (appointment.TrainerAvailability.IsFull)
                {
                    appointment.TrainerAvailability.IsFull = false;
                    _context.Update(appointment.TrainerAvailability);
                }
            }

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Info"] = "Randevunuz iptal edildi.";
            return RedirectToAction(nameof(MyAppointments));
        }

        // GET: Trainers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TrainerId,FullName,Email,Phone,Bio,Expertise")] Trainer trainer, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/img/trainers/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    trainer.ImageUrl = "/img/trainers/" + fileName;
                }
                else
                {
                    trainer.ImageUrl = "/img/trainers/default_coach.png";
                }

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,FullName,Email,Phone,Bio,Expertise,ImageUrl")] Trainer trainer, IFormFile? imageFile)
        {
            if (id != trainer.TrainerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/img/trainers/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        if (!string.IsNullOrEmpty(trainer.ImageUrl))
                        {
                            var oldPath = Path.Combine(wwwRootPath, trainer.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }

                        trainer.ImageUrl = "/img/trainers/" + fileName;
                    }
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers.FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                if (!string.IsNullOrEmpty(trainer.ImageUrl))
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    var imagePath = Path.Combine(wwwRootPath, trainer.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }
                _context.Trainers.Remove(trainer);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.TrainerId == id);
        }

        // GET: Trainers/CreateAvailability
        [Authorize(Roles = "Trainer")]
        public IActionResult CreateAvailability()
        {
            // Hizmet listesini View'a gönderiyoruz (Dropdown için)
            ViewData["FitnessServiceId"] = new SelectList(_context.FitnessServices, "FitnessServiceId", "ServiceName");
            return View();
        }

        // POST: Trainers/CreateAvailability
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> CreateAvailability([Bind("Date,StartTime,EndTime,Capacity,FitnessServiceId")] TrainerAvailability model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var trainerProfile = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == user.Email);

            if (trainerProfile == null)
            {
                ModelState.AddModelError("", "Email adresinize tanımlı bir antrenör profili yok.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                model.TrainerId = trainerProfile.TrainerId;
                model.IsFull = false; // İlk oluştuğunda boş
                                      // Kapasite formdan gelmezse modeldeki varsayılan (1) geçerli olur.

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = trainerProfile.TrainerId });
            }
            ViewData["FitnessServiceId"] = new SelectList(_context.FitnessServices, "FitnessServiceId", "ServiceName", model.FitnessServiceId);

            return View(model);
        }

        // Eğitmenlerin Randevu Yönetim İşlemleri

        // Randevuları Listeleme Sayfası
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> ManageAppointments()
        {
            // Giriş yapan eğitmeni bul
            var user = await _userManager.GetUserAsync(User);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == user.Email);

            if (trainer == null) return NotFound("Eğitmen profili bulunamadı.");

            // Bu eğitmene ait randevuları getir
            var appointments = await _context.Appointments
                .Include(a => a.MemberUser)
                .Include(a => a.FitnessService)
                .Where(a => a.TrainerId == trainer.TrainerId)
                .OrderByDescending(a => a.StartTime) 
                .ToListAsync();

            return View(appointments);
        }

        // Randevu Onaylama İşlemi
        [HttpPost]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            // Durumu güncelle
            appointment.Status = AppointmentStatus.Confirmed;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevu onaylandı.";
            return RedirectToAction(nameof(ManageAppointments));
        }

        // Randevu Reddetme/İptal İşlemi
        [HttpPost]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.TrainerAvailability) // Kontenjanı geri açmak için buna ihtiyaç var
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            // Durumu güncelle
            appointment.Status = AppointmentStatus.Cancelled;

            // ÖNEMLİ: Randevu iptal edilirse, o saatteki kontenjan (IsFull) geri açılmalı!
            if (appointment.TrainerAvailability != null)
            {
                // Eğer slot "Tamamen Dolu" işaretlendiyse, artık yer açıldığı için false yapar.
                if (appointment.TrainerAvailability.IsFull)
                {
                    appointment.TrainerAvailability.IsFull = false;
                    _context.Update(appointment.TrainerAvailability);
                }
                // Not: Appointment tablosunda durmaya devam eder ama durumu "Cancelled" olur.
            }

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Info"] = "Randevu reddedildi/iptal edildi.";
            return RedirectToAction(nameof(ManageAppointments));
        }
    }
}