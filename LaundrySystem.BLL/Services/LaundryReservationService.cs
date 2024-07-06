namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Services.Interfaces;
    using LaundrySystem.BLL.SMS;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Interfaces;
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.Extensions.Logging;

    public class LaundryReservationService : BaseService<LaundryReservationModel, LaundryReservation, ILaundryReservationRepo>, ILaundryReservationService
    {
        private readonly ISMSService _smsService;
        private readonly IHouseholdService _householdService;

        public LaundryReservationService(ILaundryReservationRepo repository, ILogger<BaseService<LaundryReservationModel, LaundryReservation, ILaundryReservationRepo>> logger, ISMSService smsService, IHouseholdService householdService) : base(repository, logger)
        {
            _smsService = smsService;
            _householdService = householdService;
        }

        public void SendReminder(int reservationId)
        {
            var reservationResponse = GetById(reservationId);
            if (!reservationResponse.Success || reservationResponse.Data == null)
            {
                // Handle case where reservation is not found
                Logger.LogError("Reservation not found for ID {ReservationId}", reservationId);
                return;
            }

            var reservation = reservationResponse.Data;
            var householdResponse = _householdService.GetById(reservation.HouseholdId);
            if (!householdResponse.Success || householdResponse.Data == null)
            {
                // Handle case where household is not found
                Logger.LogError("Household not found for reservation ID {ReservationId}", reservationId);
                return;
            }

            var household = householdResponse.Data;
            var phoneNumber1 = household.Addresses.FirstOrDefault()?.PhoneNumber1; // not null
            var phoneNumber2 = household.Addresses.FirstOrDefault()?.PhoneNumber2; // nullable

            var message = $"Reminder: You have a laundry reservation at {reservation.ExpectedStart}.";

            // Send SMS to PhoneNumber1
            if (!string.IsNullOrEmpty(phoneNumber1))
            {
                _smsService.SendSMS(phoneNumber1, message);
            }
            else
            {
                // Log error if PhoneNumber1 is unexpectedly missing
                Logger.LogError("Phone number 1 not found for household ID {HouseholdId}", household.HouseholdId);
            }

            // Send SMS to PhoneNumber2 if it exists
            if (!string.IsNullOrEmpty(phoneNumber2))
            {
                _smsService.SendSMS(phoneNumber2, message);
            }
        }
    }
}