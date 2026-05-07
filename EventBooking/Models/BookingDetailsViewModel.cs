namespace EventBooking.Models
{
    public class BookingDetailsViewModel
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }

        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string? VenueImageUrl { get; set; }
    }
}
