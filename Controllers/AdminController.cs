using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaFit.Data; // Veritabanı bağlantısı için
using System.Linq;

namespace NovaFit.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin girebilir
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Bazı istatistikleri çekelim
            ViewBag.TrainerCount = _context.Trainers.Count();
            ViewBag.ServiceCount = _context.FitnessServices.Count();
            ViewBag.UserCount = _context.Users.Count();

            return View();
        }
    }
}