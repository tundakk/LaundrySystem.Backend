namespace LaundrySystem.Domain.Model.Models
{
    using System;
    using LaundrySystem.Domain.Model.Entities;

    public class ServiceMessageModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Severity { get; set; } = "info";
        public DateTime ActiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? ActiveTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? BuildingId { get; set; }
    }
}
