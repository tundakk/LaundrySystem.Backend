namespace LaundrySystem.Domain.Model.Models
{
    public class AddressModel
    {
        public int AddressId { get; set; }
        public string HomeAddress { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string EmailAddress { get; set; }
        public int HouseholdId { get; set; }
        public HouseholdModel Household { get; set; }
    }
}