namespace LaundrySystem.BLL.Infrastructure.Interfaces
{
    using LaundrySystem.Domain.Model.Models;
    using LaundrySystem.Domain.Model.Responses;

    /// <summary>
    /// Provides methods for managing bookings.
    /// </summary>
    public interface IBookingService : IBaseService<BookingModel>
    {
        /// <summary>
        /// Creates a new booking for the specified user and timeslot.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="timeslotId">The timeslot ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A service response containing the created booking model.</returns>
        Task<ServiceResponse<BookingModel>> CreateBookingAsync(Guid userId, Guid timeslotId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets upcoming bookings for a user with timeslot and room details.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A service response containing enriched booking data.</returns>
        Task<ServiceResponse<IEnumerable<object>>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}