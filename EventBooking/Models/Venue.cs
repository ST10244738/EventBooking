using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        public string VenueName { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public ICollection<Event>? Events { get; set; }
    }
}
