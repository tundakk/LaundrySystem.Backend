namespace LaundrySystem.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [MaxLength(100)]
        public string HomeAddress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(20)]
        public string PhoneNumber1 { get; set; }

        [DataType(DataType.PhoneNumber)]
        [MaxLength(20)]
        public string? PhoneNumber2 { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        public int HouseholdId { get; set; }
        public Household Household { get; set; }
    }
}