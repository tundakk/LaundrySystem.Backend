namespace LaundrySystem.Domain.Model.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AppUserTabletModel
    {
        [Required(ErrorMessage = "Apartment number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Apartment number must be greater than 0.")]
        public int ApartmentNumber { get; set; }

        [Range(1000, 9999, ErrorMessage = "Pin code must be a 4-digit number.")]
        public int? PinCode { get; set; }
    }
}