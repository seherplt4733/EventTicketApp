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
            // --- Kart İstatistikleri ---
            ViewBag.TotalEvents = await _context.Events.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.UpcomingEvents = await _context.Events.CountAsync(e => e.EventDate >= DateTime.Now);
            ViewBag.AveragePrice = await _context.Events.AnyAsync() ? await _context.Events.AverageAsync(e => e.Price) : 0;

            // --- Chart 1: Kategoriye Göre Etkinlik Dağılımı (Doughnut Chart) ---
            var categoryData = await _context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    EventCount = c.Events.Count()
                }).ToListAsync();

            ViewBag.CategoryLabels = categoryData.Select(c => c.CategoryName).ToArray();
            ViewBag.CategoryCounts = categoryData.Select(c => c.EventCount).ToArray();

            // --- Chart 2: Satılan Bilet Sayılarına Göre Top Etkinlikler (Bar Chart) ---
            var ticketData = await _context.Events
                .OrderByDescending(e => e.SoldTicketsCount)
                .Take(5)
                .Select(e => new
                {
                    EventTitle = e.Title,
                    Solds = e.SoldTicketsCount
                }).ToListAsync();

            ViewBag.EventLabels = ticketData.Select(t => t.EventTitle).ToArray();
            ViewBag.EventSolds = ticketData.Select(t => t.Solds).ToArray();

            // Son eklenen 3 etkinlik
            var latestEvents = await _context.Events
                .Include(e => e.Category)
                .OrderByDescending(e => e.Id)
                .Take(3)
                .ToListAsync();

            return View(latestEvents);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}