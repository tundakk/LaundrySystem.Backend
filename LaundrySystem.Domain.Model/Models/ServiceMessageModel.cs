namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class ServiceMessageModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}