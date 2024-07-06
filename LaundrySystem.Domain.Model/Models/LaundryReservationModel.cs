namespace LaundrySystem.Domain.Model.Models
{
    using System;

    public class LaundryReservationModel
    {
        public int ReservationId { get; set; }
        public int HouseholdId { get; set; }
        public int SlotId { get; set; }
        public TimeSpan ExpectedStart { get; set; }
        public HouseholdModel Household { get; set; }
        public SlotModel Slot { get; set; }
    }
}