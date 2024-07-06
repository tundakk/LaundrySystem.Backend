namespace LaundrySystem.DAL.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class LostAndFound
    {
        [Key]
        public int LostAndFoundId { get; set; }

        public int HouseholdId { get; set; }
        public byte[] Image { get; set; }
        public string TextMessage { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Household Household { get; set; }
    }
}