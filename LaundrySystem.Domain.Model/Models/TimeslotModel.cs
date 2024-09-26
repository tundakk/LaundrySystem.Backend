namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class TimeslotModel
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; } // Foreign key to Room
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}