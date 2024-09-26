using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            // Corrected relationship configuration
            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings) // Specify the navigation property here
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Timeslot)
                .WithMany(t => t.Bookings) // Ensure Timeslot has a Bookings collection
                .HasForeignKey(b => b.TimeslotId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Seed data
            //var bookingId = Guid.NewGuid();

            //builder.HasData(
            //    new Booking
            //    {
            //        Id = bookingId,
            //        UserId = Guid.Parse("115b5117-73f6-4796-a87a-962181baa3e5"), // User1
            //        TimeslotId = Guid.Parse("2e7c8c0d-9f6b-4e4f-9f4b-2a1d9f6b4e4f"), // Corrected timeslot1Id
            //        Status = BookingStatus.Pending,
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow
            //    }
            //);
        }
    }
}