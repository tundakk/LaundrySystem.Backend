namespace LaundrySystem.DAL.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ChatMessage
    {
        [Key]
        public int ChatMessageId { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string SenderId { get; set; }  // Foreign key to AspNetUsers

        [Required]
        [ForeignKey("Household")]
        public int HouseholdId { get; set; }

        public Household Household { get; set; }
    }
}