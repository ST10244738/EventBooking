using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventBooking.Data;
using EventBooking.Models;

namespace EventBooking.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Search by BookingId (numeric) or Event Name (text)
                if (int.TryParse(search, out int bookingId))
                    query = query.Where(b => b.BookingId == bookingId);
                else
                    query = query.Where(b => b.Event.EventName.Contains(search));
            }

            var results = await query.Select(b => new BookingDetailsViewModel
            {
                BookingId   = b.BookingId,
                BookingDate = b.BookingDate,
                EventId     = b.EventId,
                EventName   = b.Event.EventName,
                EventDate   = b.Event.EventDate,
                Description = b.Event.Description,
                VenueId     = b.VenueId,
                VenueName   = b.Venue.VenueName,
                Location    = b.Venue.Location,
                Capacity    = b.Venue.Capacity,
                VenueImageUrl = b.Venue.ImageUrl
            }).ToListAsync();

            ViewBag.Search = search;
            return View(results);
        }

        public IActionResult Create()
        {
            ViewBag.Events = new SelectList(_context.Events, "EventId", "EventName");
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (booking.EventId == 0 || booking.VenueId == 0 || booking.BookingDate == default)
            {
                ModelState.AddModelError("", "All fields are required.");
            }

            // Prevent double-booking: same venue on the same date
            bool conflict = _context.Bookings.Any(b =>
                b.VenueId == booking.VenueId &&
                b.BookingDate.Date == booking.BookingDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked on that date. Please choose a different date or venue.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Events = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
