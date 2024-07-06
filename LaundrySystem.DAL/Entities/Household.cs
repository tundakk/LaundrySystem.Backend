namespace LaundrySystem.DAL.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Household
    {
        [Key]
        public int HouseholdId { get; set; }

        [Required]
        public string UserId { get; set; }  // Foreign key to AspNetUsers

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Navigation properties
        public ICollection<Address> Addresses { get; set; }

        public ICollection<Slot> Slots { get; set; }
        public ICollection<LaundryReservation> LaundryReservations { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
        public ICollection<LostAndFound> LostAndFoundItems { get; set; }
    }
}