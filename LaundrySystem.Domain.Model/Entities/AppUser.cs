using LaundrySystem.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser<Guid>
{
    public int ApartmentNumber { get; set; }
    public string? PhoneNumberSecondary { get; set; }
    public bool EmailOptOut { get; set; } = false;
    public bool SmsOptOut { get; set; } = false;
    public int? PinCode { get; set; }

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public ICollection<DesiredTimeslot> DesiredTimeslots { get; set; } = new List<DesiredTimeslot>();

    // Business logic methods
    public void SetPinCode(int pinCode)
    {
        if (pinCode < 1000 || pinCode > 9999)
            throw new ArgumentException("PIN must be a 4-digit number.");

        PinCode = pinCode;
    }
}