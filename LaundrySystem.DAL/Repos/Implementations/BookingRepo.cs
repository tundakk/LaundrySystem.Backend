using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos.Base;
using LaundrySystem.DAL.Repos.Interfaces;
using LaundrySystem.Domain.Model.Entities;
using LaundrySystem.Domain.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.DAL.Repos
{
    public class BookingRepo : BaseRepo<Booking>, IBookingRepo
    {
        public BookingRepo(DataContext dataContext) : base(dataContext)
        {
        }

        // Implement additional methods specific to Booking if needed
        public async Task<bool> HasActiveBookingAsync(Guid userId)
        {
            var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.InProgress };

            return await dataContext.Bookings
                .AnyAsync(b => b.UserId == userId && activeStatuses.Contains(b.Status));
        }
    }
}