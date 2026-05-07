using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a venue.")]
        public int VenueId { get; set; }

        public Venue? Venue { get; set; }
    }
}