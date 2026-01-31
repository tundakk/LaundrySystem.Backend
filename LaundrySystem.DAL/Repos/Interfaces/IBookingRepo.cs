namespace LaundrySystem.DAL.Repos.Interfaces
{
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.Domain.Model.Entities;

    public interface IBookingRepo : IBaseRepo<Booking>
    {
        /// <summary>
        /// Checks if a user has an active (pending) booking.
        /// </summary>
        Task<bool> HasActiveBookingAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets bookings that need reminder notifications sent.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsNeedingReminderAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets recent bookings for a building (for lost and found notifications).
        /// </summary>
        Task<IEnumerable<Booking>> GetRecentByBuildingAsync(Guid buildingId, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets upcoming bookings for a user, including timeslot and room details.
        /// </summary>
        Task<IEnumerable<Booking>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}