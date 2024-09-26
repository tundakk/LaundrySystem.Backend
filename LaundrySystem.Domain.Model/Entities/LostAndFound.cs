namespace LaundrySystem.Domain.Model.Entities
{
    using System;

    public class LostAndFound
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PictureUrl { get; set; } // Azure Blob Storage URL
        public DateTime DateFound { get; set; } = DateTime.UtcNow;
    }
}