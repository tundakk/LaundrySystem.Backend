namespace LaundrySystem.Domain.Model.Models
{
    using System;

    /// <summary>
    /// Seperations of concern. dont make db specifics as part of the domain models such as attributes like [Key] or [Required]
    /// </summary>
    public class ChatMessageModel
    {
        public int ChatMessageId { get; set; }
        public int HouseholdId { get; set; }
        public int SenderId { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public HouseholdModel Household { get; set; }
    }
}