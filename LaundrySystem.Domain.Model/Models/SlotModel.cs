namespace LaundrySystem.Domain.Model.Models
{
    using System;
    using System.Collections.Generic;

    public class SlotModel
    {
        public int SlotId { get; set; }
        public DateTime AvailableDateTime { get; set; }
        public bool Reserved { get; set; }
        public int? NotifyHouseholdId { get; set; }
        public HouseholdModel NotifyHousehold { get; set; }
        public ICollection<LaundryReservationModel> LaundryReservations { get; set; } = new List<LaundryReservationModel>();
    }
}