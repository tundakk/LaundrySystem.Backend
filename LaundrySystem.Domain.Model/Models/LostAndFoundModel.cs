namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class LostAndFoundModel
    {
        public int LostAndFoundId { get; set; }
        public int HouseholdId { get; set; }
        public byte[] Image { get; set; }
        public string TextMessage { get; set; }
        public DateTime RegistrationDate { get; set; }
        public HouseholdModel Household { get; set; }
    }
}