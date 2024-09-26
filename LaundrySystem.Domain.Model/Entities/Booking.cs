using LaundrySystem.Domain.Model.Enums;

namespace LaundrySystem.Domain.Model.Entities
{
    public class Booking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid TimeslotId { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public AppUser User { get; set; }

        public Timeslot Timeslot { get; set; }

        // Business logic methods
        public void MarkAsCompleted()
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidOperationException("Only pending bookings can be marked as completed.");

            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == BookingStatus.Completed)
                throw new InvalidOperationException("Completed bookings cannot be canceled.");

            Status = BookingStatus.Canceled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}