namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class ServiceMessageModel
    {
        public int ServiceMessageId { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}