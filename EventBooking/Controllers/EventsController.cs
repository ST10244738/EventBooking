using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;

namespace EventBooking.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = _context.Events.Include(e => e.Venue);
            return View(await events.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event e)
        {
            if (string.IsNullOrEmpty(e.EventName) || e.EventDate == default)
            {
                ModelState.AddModelError("", "All fields are required.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(e);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", e.VenueId);
            return View(e);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var e = await _context.Events.FindAsync(id);
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", e.VenueId);
            return View(e);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event e)
        {
            if (ModelState.IsValid)
            {
                _context.Update(e);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(e);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var e = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(x => x.EventId == id);

            if (e == null) return NotFound();

            return View(e);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var e = await _context.Events.FindAsync(id);
            if (e == null) return NotFound();

            var bookings = _context.Bookings.Where(b => b.EventId == id);
            _context.Bookings.RemoveRange(bookings);

            _context.Events.Remove(e);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}