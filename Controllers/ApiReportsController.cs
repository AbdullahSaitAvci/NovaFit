using Microsoft.AspNetCore.Mvc;
using NovaFit.Data;
using System.Linq;

namespace NovaFit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiReports/TrainerStats
        [HttpGet("TrainerStats")]
        public IActionResult GetTrainerStats()
        {
            // LINQ Sorgusu: Eğitmenleri ve randevu sayılarını getirir 
            var stats = _context.Trainers
                .Select(t => new {
                    EgitmenAdi = t.FullName,
                    Uzmanlik = t.Expertise,
                    ToplamRandevu = t.TrainerAvailabilities
                        .SelectMany(a => a.Appointments)
                        .Count(app => app.Status == Models.AppointmentStatus.Completed)
                })
                .OrderByDescending(x => x.ToplamRandevu)
                .ToList();

            return Ok(stats);
        }
    }
}