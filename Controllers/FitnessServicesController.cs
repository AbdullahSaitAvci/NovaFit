using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NovaFit.Controllers
{
    public class FitnessServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FitnessServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FitnessServices
        public async Task<IActionResult> Index()
        {
            return View(await _context.FitnessServices.ToListAsync());
        }

        // GET: FitnessServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessService = await _context.FitnessServices
                .FirstOrDefaultAsync(m => m.FitnessServiceId == id);
            if (fitnessService == null)
            {
                return NotFound();
            }

            return View(fitnessService);
        }

        // GET: FitnessServices/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: FitnessServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FitnessServiceId,ServiceName,Description,DurationMinutes,Price")] FitnessService fitnessService)
        {
            if (fitnessService.DurationMinutes % 15 != 0)
            {
                // Hatayı ModelState'e ekliyoruz. Sayfa tekrar yüklendiğinde görünecek.
                ModelState.AddModelError("DurationMinutes", "Hizmet süresi 15 dakikanın katları olmalıdır (15, 30, 45...).");
            }

            if (ModelState.IsValid)
            {
                _context.Add(fitnessService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessService); 
        }

        // GET: FitnessServices/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessService = await _context.FitnessServices.FindAsync(id);
            if (fitnessService == null)
            {
                return NotFound();
            }
            return View(fitnessService);
        }

        // POST: FitnessServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("FitnessServiceId,ServiceName,Description,DurationMinutes,Price")] FitnessService fitnessService)
        {
            if (fitnessService.DurationMinutes % 15 != 0)
            {
                // Hatayı ModelState'e ekliyoruz. Sayfa tekrar yüklendiğinde görünecek.
                ModelState.AddModelError("DurationMinutes", "Hizmet süresi 15 dakikanın katları olmalıdır (15, 30, 45...).");
            }

            if (id != fitnessService.FitnessServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fitnessService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessServiceExists(fitnessService.FitnessServiceId))
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
            return View(fitnessService);
        }

        // GET: FitnessServices/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessService = await _context.FitnessServices
                .FirstOrDefaultAsync(m => m.FitnessServiceId == id);
            if (fitnessService == null)
            {
                return NotFound();
            }

            return View(fitnessService);
        }

        // POST: FitnessServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessService = await _context.FitnessServices.FindAsync(id);
            if (fitnessService != null)
            {
                _context.FitnessServices.Remove(fitnessService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FitnessServiceExists(int id)
        {
            return _context.FitnessServices.Any(e => e.FitnessServiceId == id);
        }
    }
}
