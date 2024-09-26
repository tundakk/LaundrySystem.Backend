namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class LostAndFoundModel
    {
        public Guid Id { get; set; }
        public string PictureUrl { get; set; } // URL to the picture
        public DateTime DateFound { get; set; }
    }
}