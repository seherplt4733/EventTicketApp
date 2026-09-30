using EventTicketApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventTicketApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikler
            ViewBag.TotalEvents = await _context.Events.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();

            ViewBag.UpcomingEvents = await _context.Events
                .Where(e => e.EventDate > DateTime.Now)
                .CountAsync();

            ViewBag.AveragePrice = await _context.Events.AnyAsync()
                ? await _context.Events.AverageAsync(e => e.Price)
                : 0;

            // Son 5 etkinlik
            var recentEvents = await _context.Events
                .Include(e => e.Category)
                .OrderByDescending(e => e.Id)
                .Take(5)
                .ToListAsync();

            return View(recentEvents);
        }
    }
}