using System.ComponentModel.DataAnnotations;

namespace LaundrySystem.Domain.Model.Models
{
    public class AppUserUpdateModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is not valid.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Apartment number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Apartment number must be greater than 0.")]
        public int ApartmentNumber { get; set; }

        [Phone(ErrorMessage = "Secondary phone number is not valid.")]
        public string? PhoneNumberSecondary { get; set; }

        public bool EmailOptOut { get; set; }
        public bool SmsOptOut { get; set; }

        [Range(1000, 9999, ErrorMessage = "Pin code must be a 4-digit number.")]
        public int? PinCode { get; set; }
    }
}