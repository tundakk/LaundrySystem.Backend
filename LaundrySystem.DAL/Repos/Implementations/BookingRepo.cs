using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos.Base;
using LaundrySystem.DAL.Repos.Interfaces;
using LaundrySystem.Domain.Model.Entities;
using LaundrySystem.Domain.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LaundrySystem.DAL.Repos
{
    public class BookingRepo : BaseRepo<Booking>, IBookingRepo
    {
        public BookingRepo(DataContext dataContext, ILogger<BookingRepo> logger) : base(dataContext, logger)
        {
        }

        public async Task<bool> HasActiveBookingAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.InProgress };

            return await dataContext.Bookings
                .AnyAsync(b => b.UserId == userId && activeStatuses.Contains(b.Status), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingsNeedingReminderAsync(CancellationToken cancellationToken = default)
        {
            // Default reminder window: 24 hours before
            var reminderHours = 24;
            var now = DateTime.UtcNow;
            var reminderWindowEnd = now.AddHours(reminderHours);

            return await dataContext.Bookings
                .Include(b => b.User)
                .Include(b => b.Timeslot)
                    .ThenInclude(t => t.Room)
                .Where(b =>
                    b.Status == BookingStatus.Pending &&
                    !b.ReminderSent &&
                    b.Timeslot.SlotTime.Start > now &&
                    b.Timeslot.SlotTime.Start <= reminderWindowEnd)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetRecentByBuildingAsync(Guid buildingId, int count, CancellationToken cancellationToken = default)
        {
            return await dataContext.Bookings
                .Include(b => b.User)
                .Include(b => b.Timeslot)
                    .ThenInclude(t => t.Room)
                .Where(b => b.Timeslot.Room.BuildingId == buildingId)
                .OrderByDescending(b => b.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.InProgress };

            return await dataContext.Bookings
                .Include(b => b.Timeslot)
                    .ThenInclude(t => t.Room)
                .Where(b => b.UserId == userId && activeStatuses.Contains(b.Status))
                .OrderBy(b => b.Timeslot.SlotTime.Start)
                .ToListAsync(cancellationToken);
        }
    }
}