using EventTicketApp.Data;
using EventTicketApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventTicketApp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Tickets/Buy/5
        public async Task<IActionResult> Buy(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();

            if (@event.AvailableSeats <= 0)
            {
                TempData["Error"] = "Üzgünüz, bu etkinliğin biletleri tükenmiştir.";
                return RedirectToAction("Details", "Events", new { id = id });
            }

            var ticketModel = new Ticket
            {
                EventId = @event.Id,
                Event = @event,
                Quantity = 1
            };

            return View(ticketModel);
        }

        // POST: /Tickets/Buy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(Ticket ticket)
        {
            var @event = await _context.Events.FindAsync(ticket.EventId);

            if (@event == null) return NotFound();

            if (ticket.Quantity > @event.AvailableSeats)
            {
                ModelState.AddModelError("Quantity", $"Mevcut bilet sayısından fazla alamazsınız. Kalan bilet: {@event.AvailableSeats}");
            }

            if (ModelState.IsValid)
            {
                // Toplam fiyat hesabı ve bilet kodu üretimi
                ticket.TotalPrice = ticket.Quantity * @event.Price;
                ticket.PurchaseDate = DateTime.Now;
                ticket.TicketCode = "TKT-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

                // Etkinliğin satılan bilet sayısını güncelle
                @event.SoldTicketsCount += ticket.Quantity;

                _context.Add(ticket);
                _context.Update(@event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation), new { id = ticket.Id });
            }

            ticket.Event = @event;
            return View(ticket);
        }

        // GET: /Tickets/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .ThenInclude(e => e!.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }



        // GET: /Tickets/ExportToCsv
[HttpGet]
public async Task<IActionResult> ExportToCsv()
{
    var tickets = await _context.Tickets
        .Include(t => t.Event)
        .OrderByDescending(t => t.PurchaseDate)
        .ToListAsync();

    var builder = new System.Text.StringBuilder();

    // CSV Başlık Satırı (Türkçe karakterlerin Excel'de düzgün görünmesi için UTF-8 BOM ekliyoruz)
    builder.AppendLine("Bilet Kodu;Müşteri Adı;E-Posta;Etkinlik;Adet;Toplam Tutar;Tarih");

    foreach (var ticket in tickets)
    {
        builder.AppendLine($"{ticket.TicketCode};{ticket.CustomerName};{ticket.CustomerEmail};{ticket.Event?.Title};{ticket.Quantity};{ticket.TotalPrice:N2};{ticket.PurchaseDate:dd.MM.yyyy HH:mm}");
    }

    // Encoding: UTF8 ile Türkçe karakter sorunu yaşanmaz
    var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(builder.ToString())).ToArray();

    return File(bytes, "text/csv", $"Bilet_Satis_Raporu_{DateTime.Now:yyyyMMdd}.csv");
}
    }
}