using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EF;
using EventTicketApp.Data;
using EventTicketApp.Models;

namespace EventTicketApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = page ?? 1;

            var events = await _context.Events
                .Include(e => e.Category)
                .OrderBy(e => e.EventDate)
                .ToPagedListAsync(pageNumber, pageSize);

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // myEvent değişken ismi tek tip hale getirildi
            var myEvent = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (myEvent == null)
            {
                return NotFound();
            }

            return View(myEvent);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event myEvent, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    myEvent.ImagePath = await SaveImageAsync(imageFile);
                }

                _context.Add(myEvent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Etkinlik başarıyla eklendi! 🎉";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", myEvent.CategoryId);
            return View(myEvent);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myEvent = await _context.Events.FindAsync(id);
            if (myEvent == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", myEvent.CategoryId);
            return View(myEvent);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event myEvent, IFormFile? imageFile)
        {
            if (id != myEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        myEvent.ImagePath = await SaveImageAsync(imageFile);
                    }
                    else
                    {
                        var existing = await _context.Events.AsNoTracking()
                            .FirstOrDefaultAsync(e => e.Id == id);
                        if (existing != null)
                        {
                            myEvent.ImagePath = existing.ImagePath;
                        }
                    }

                    _context.Update(myEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.Id == myEvent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Etkinlik başarıyla güncellendi! ✏️";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", myEvent.CategoryId);
            return View(myEvent);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myEvent = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (myEvent == null)
            {
                return NotFound();
            }

            return View(myEvent);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myEvent = await _context.Events.FindAsync(id);
            if (myEvent != null)
            {
                _context.Events.Remove(myEvent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Etkinlik başarıyla silindi! 🗑️";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Events/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int eventId, string userName, string comment, int rating)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Lütfen tüm alanları doldurun.";
                return RedirectToAction(nameof(Details), new { id = eventId });
            }

            var review = new Review
            {
                EventId = eventId,
                UserName = userName,
                Comment = comment,
                Rating = Math.Clamp(rating, 1, 5),
                CreatedDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Yorumunuz başarıyla eklendi!";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        // YARDIMCI METOT: Resim Kaydetme
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "events");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/images/events/{fileName}";
        }
    }
}