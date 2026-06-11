using Microsoft.EntityFrameworkCore;
using EventBooking.Models;

namespace EventBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany()
                .HasForeignKey(b => b.VenueId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed predefined event type categories
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, Name = "Conference" },
                new EventType { EventTypeId = 2, Name = "Wedding" },
                new EventType { EventTypeId = 3, Name = "Concert" },
                new EventType { EventTypeId = 4, Name = "Birthday" },
                new EventType { EventTypeId = 5, Name = "Corporate" },
                new EventType { EventTypeId = 6, Name = "Exhibition" },
                new EventType { EventTypeId = 7, Name = "Other" }
            );
        }
    }
}