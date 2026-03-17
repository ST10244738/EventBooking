using Microsoft.AspNetCore.Mvc;
using EventBooking.Data;

namespace EventBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalVenues = _context.Venues.Count();
            ViewBag.TotalEvents = _context.Events.Count();
            ViewBag.TotalBookings = _context.Bookings.Count();

            return View();
        }
    }
}