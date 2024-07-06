namespace LaundrySystem.DAL.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LaundryReservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public TimeSpan ExpectedStart { get; set; }

        [ForeignKey("Household")]
        public int HouseholdId { get; set; }

        public Household Household { get; set; }

        [ForeignKey("Slot")]
        public int SlotId { get; set; }

        public Slot Slot { get; set; }
    }
}