using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;

namespace NovaFit.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context; // _context, veri tabanındaki antrenör verilerine erişmek için kullanılır
        private readonly IWebHostEnvironment _hostEnvironment; // dosya yükleme için

        public TrainersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment; 
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
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
                // Resim Yükleme İşlemi 
                if (imageFile != null)
                {
                    // 1. Resim için benzersiz bir isim oluştur (Örn: ali-veli-guid.jpg)
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);

                    // Dosya ismini temizle ve benzersiz yap
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    // Kaydedilecek yer: wwwroot/img/trainers/resim.jpg
                    string path = Path.Combine(wwwRootPath + "/img/trainers/", fileName);

                    // 2. Dosyayı fiziksel olarak klasöre kopyala
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // 3. Veritabanına sadece dosya yolunu yaz
                    trainer.ImageUrl = "/img/trainers/" + fileName;
                }
                else
                {
                    // Resim yüklenmediyse varsayılan bir resim ata (Opsiyonel)
                    trainer.ImageUrl = "/img/default-user.png";
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
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,FullName,Email,Phone,Bio,Expertise,ImageUrl")] Trainer trainer, IFormFile? imageFile)
        {
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Resim Güncelleme İşlemi
                    if (imageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/img/trainers/", fileName);

                        // Yeni resmi kaydet
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Eğer eski bir resim varsa ve varsayılan değilse, onu diskten sil
                        if (!string.IsNullOrEmpty(trainer.ImageUrl))
                        {
                            var oldPath = Path.Combine(wwwRootPath, trainer.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Yeni yolu ata
                        trainer.ImageUrl = "/img/trainers/" + fileName;
                    }
                    // --------------------------------

                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

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
                // --- RESİM SİLME İŞLEMİ ---
                // Eğer antrenörün resmi varsa, klasörden de sil
                if (!string.IsNullOrEmpty(trainer.ImageUrl))
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    // Resim yolunun başındaki "/" işaretini kaldırıp tam yolu buluyoruz
                    var imagePath = Path.Combine(wwwRootPath, trainer.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
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
    }
}
