namespace LaundrySystem.Domain.Model.Entities
{
    using LaundrySystem.Domain.Model.ValueObjects;

    public class Timeslot
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RoomId { get; set; }

        public TimeRange SlotTime { get; set; }

        //public bool IsAvailable { get; private set; }
        public bool IsAvailable { get; set; }

        //public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        //public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Room Room { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<DesiredTimeslot> DesiredTimeslots { get; set; } = new List<DesiredTimeslot>();

        // Business logic methods
        public void MarkAsAvailable()
        {
            IsAvailable = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsUnavailable()
        {
            IsAvailable = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOverlapping(TimeRange otherTimeRange)
        {
            return SlotTime.OverlapsWith(otherTimeRange);
        }
    }
}