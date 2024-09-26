namespace LaundrySystem.Domain.Model.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AppUserModel
    {
        public Guid Id { get; set; }  // User ID (from IdentityUser)

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public int ApartmentNumber { get; set; }

        public string? PhoneNumberSecondary { get; set; }
        public bool EmailOptOut { get; set; }
        public bool SmsOptOut { get; set; }
        public int? PinCode { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }
    }
}