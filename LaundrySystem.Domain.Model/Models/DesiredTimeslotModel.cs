namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class DesiredTimeslotModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // Foreign key to AppUser
        public Guid TimeslotId { get; set; } // Foreign key to Timeslot
        public bool NotificationSent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}