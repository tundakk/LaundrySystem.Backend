using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.DAL.Repos.Interfaces;
using LaundrySystem.Domain.Model.Entities;
using LaundrySystem.Domain.Model.Enums;
using LaundrySystem.Domain.Model.Models;
using LaundrySystem.Domain.Model.Responses;
using Mapster; // Ensure Mapster is included for mapping
using Microsoft.Extensions.Logging;

namespace LaundrySystem.BLL.Infrastructure.Services.Implementations
{
    /// <summary>
    /// Service for handling bookings.
    /// </summary>
    public class BookingService : BaseService<BookingModel, Booking, IBookingRepo>, IBookingService
    {
        private readonly ITimeslotRepo _timeslotRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="bookingRepo">The booking repository.</param>
        /// <param name="timeslotRepo">The timeslot repository.</param>
        /// <param name="logger">The logger instance.</param>
        public BookingService(IBookingRepo bookingRepo, ITimeslotRepo timeslotRepo, ILogger<BookingService> logger)
            : base(bookingRepo, logger)
        {
            _timeslotRepo = timeslotRepo;
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="timeslotId">The timeslot ID.</param>
        /// <returns>The created booking model.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user already has an active booking or the timeslot is not available.</exception>
        public async Task<BookingModel> CreateBookingAsync(Guid userId, Guid timeslotId)
        {
            // Check for active booking
            if (await Repository.HasActiveBookingAsync(userId))
            {
                throw new InvalidOperationException("User already has an active booking.");
            }

            // Check timeslot availability
            var timeslot = await _timeslotRepo.GetByIdAsync(timeslotId);
            if (timeslot == null || !timeslot.IsAvailable)
            {
                throw new InvalidOperationException("Timeslot is not available.");
            }

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                TimeslotId = timeslotId,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Insert booking
            await Repository.InsertAsync(booking);

            // Update timeslot availability
            timeslot.MarkAsUnavailable();
            await _timeslotRepo.UpdateAsync(timeslot);

            // Map Booking to BookingModel
            var bookingModel = booking.Adapt<BookingModel>();

            return bookingModel;
        }

        /// <summary>
        /// Deletes a booking by ID.
        /// </summary>
        /// <param name="id">The booking ID.</param>
        /// <returns>A service response indicating the success or failure of the operation.</returns>
        public override async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var booking = await Repository.GetByIdAsync(id);
                if (booking == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Booking not found",
                        Data = false
                    };
                }

                // Optional: Update timeslot to make it available again
                var timeslot = await _timeslotRepo.GetByIdAsync(booking.TimeslotId);
                if (timeslot != null)
                {
                    timeslot.MarkAsAvailable();
                    await _timeslotRepo.UpdateAsync(timeslot);
                }

                await Repository.DeleteAsync(booking);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Booking deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting booking");
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
    }
}