namespace LaundrySystem.Domain.Model.Models
{
    using System.Collections.Generic;

    public class HouseholdModel
    {
        public int HouseholdId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public ICollection<AddressModel> Addresses { get; set; }
        public ICollection<SlotModel> Slots { get; set; }
        public ICollection<LaundryReservationModel> LaundryReservations { get; set; }
        public ICollection<ChatMessageModel> ChatMessages { get; set; }
        public ICollection<LostAndFoundModel> LostAndFoundItems { get; set; }
    }
}