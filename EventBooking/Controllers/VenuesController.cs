using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;

namespace EventBooking.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var venues = from v in _context.Venues select v;

            if (!string.IsNullOrEmpty(search))
                venues = venues.Where(v => v.VenueName.Contains(search));

            return View(await venues.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (string.IsNullOrEmpty(venue.VenueName) ||
                string.IsNullOrEmpty(venue.Location) ||
                venue.Capacity <= 0)
            {
                ModelState.AddModelError("", "All fields are required.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Update(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);

            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            var events = _context.Events.Where(e => e.VenueId == id).ToList();

            foreach (var e in events)
            {
                var bookings = _context.Bookings.Where(b => b.EventId == e.EventId);
                _context.Bookings.RemoveRange(bookings);
            }

            _context.Events.RemoveRange(events);
            _context.Venues.Remove(venue);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}