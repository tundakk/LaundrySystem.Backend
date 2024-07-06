namespace LaundrySystem.DAL.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Slot
    {
        [Key]
        public int SlotId { get; set; }

        public DateTime AvailableDateTime { get; set; }
        public bool Reserved { get; set; }

        [ForeignKey("NotifyHousehold")]
        public int? NotifyHouseholdId { get; set; }

        public virtual Household NotifyHousehold { get; set; }
        public virtual ICollection<LaundryReservation> LaundryReservations { get; set; } = new List<LaundryReservation>();
    }
}