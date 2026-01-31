using System.ComponentModel.DataAnnotations;

namespace LaundrySystem.Domain.Model.Models
{
    /// <summary>
    /// Model for admin editing of user details.
    /// </summary>
    public class AdminEditUserModel
    {
        /// <summary>
        /// The user's display name / username.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters.")]
        public required string UserName { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public required string Email { get; set; }

        /// <summary>
        /// The user's apartment number.
        /// </summary>
        [Required(ErrorMessage = "Apartment number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Apartment number must be greater than 0.")]
        public int ApartmentNumber { get; set; }

        /// <summary>
        /// The user's role (User, AccountAdmin, SuperAdmin).
        /// </summary>
        [Required(ErrorMessage = "Role is required.")]
        public required string Role { get; set; }
    }
}
