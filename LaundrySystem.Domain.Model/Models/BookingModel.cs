namespace LaundrySystem.Domain.Model.Models
{
    using LaundrySystem.Domain.Model.Enums;

    public class BookingModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // Foreign key to AppUser
        public Guid TimeslotId { get; set; } // Foreign key to Timeslot
        public BookingStatus Status { get; set; } // e.g., Pending, Completed, Canceled
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}