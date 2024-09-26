namespace LaundrySystem.Domain.Model.Entities
{
    public class DesiredTimeslot
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid TimeslotId { get; set; }

        public bool NotificationSent { get; private set; } = false;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties
        public AppUser User { get; set; }

        public Timeslot Timeslot { get; set; }

        // Business logic methods
        public void MarkNotificationAsSent()
        {
            NotificationSent = true;
        }
    }
}