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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers.ToListAsync());
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

        // Randevu Alma İşlemleri
        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> RandevuAl(int id, int serviceId) // id: AvailabilityId
        {
            // Slotu ve içindeki randevuları getir
            var slot = await _context.TrainerAvailabilities
                                     .Include(x => x.Appointments)
                                     .FirstOrDefaultAsync(x => x.TrainerAvailabilityId == id);

            if (slot == null) return NotFound();

            // Kontenjan Kontrolü (IsBooked yerine kapasiteye bakıyoruz)
            if (slot.Appointments.Count >= slot.Capacity)
            {
                TempData["Error"] = "Bu dersin kontenjanı dolmuştur.";
                return RedirectToAction("Details", new { id = slot.TrainerId });
            }

            var userId = _userManager.GetUserId(User);

            // Aynı kullanıcının aynı derse tekrar kaydolmasını engeller
            if (slot.Appointments.Any(a => a.MemberUserId == userId))
            {
                TempData["Error"] = "Bu derse zaten kaydınız bulunmaktadır.";
                return RedirectToAction("Details", new { id = slot.TrainerId });
            }

            // Hizmet ücretini bul (Opsiyonel: serviceId gelmezse 0)
            decimal ucret = 0;
            if (serviceId > 0)
            {
                var service = await _context.FitnessServices.FindAsync(serviceId);
                ucret = service?.Price ?? 0;
            }

            // Yeni randevu oluşturma 
            var yeniRandevu = new Appointment
            {
                TrainerAvailabilityId = id,
                TrainerId = (int)slot.TrainerId,
                MemberUserId = userId,
                FitnessServiceId = serviceId > 0 ? serviceId : 1, // Hizmet seçilmediyse varsayılan ID (veya null kontrolü yapabilirsin)
                StartTime = slot.Date + slot.StartTime,
                EndTime = slot.Date + slot.EndTime,
                Price = ucret,
                Status = AppointmentStatus.Pending,
                CreatedDate = DateTime.Now
            };

            _context.Add(yeniRandevu);

            // Eğer kontenjan dolduysa slotu dolu işaretle
            if (slot.Appointments.Count + 1 >= slot.Capacity)
            {
                slot.IsFull = true;
                _context.Update(slot);
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevunuz başarıyla oluşturuldu!";
            return RedirectToAction(nameof(Details), new { id = slot.TrainerId });
        }
        // ==========================================

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
            return View(model);
        }
    }
}